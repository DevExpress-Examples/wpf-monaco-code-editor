using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.CodeView;
using CodeEditor;
using CodeEditor.Models;
using CodeEditor.Theming;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeEditorExample.ViewModels {
	public class MainViewModel : ViewModelBase {

		protected ObservableCollection<string> languages;

		string MyLangMonarch = @"{
  defaultToken: ""invalid"",

  keywords: [
    ""function"", ""let"", ""const"", ""if"", ""else"",
    ""return"", ""while"", ""for"", ""true"", ""false"", ""null""
  ],

  typeKeywords: [""number"", ""string"", ""boolean""],

  operators: [
    ""="", "">"", ""<"", ""!"", ""~"", ""?"", "":"", ""=="", ""<="",
    "">="", ""!="", ""&&"", ""||"", ""+"", ""-"", ""*"", ""/"", ""%""
  ],

  symbols: /[=><!~?:&|+\-*\/%]+/,

  escapes: /\\(?:[abfnrtv\\""'`]|x[0-9A-Fa-f]{2}|u[0-9A-Fa-f]{4})/,

  tokenizer: {

    root: [

      // identifiers
      [/[a-zA-Z_$][\w$]*/, {
        cases: {
          ""@keywords"": ""keyword"",
          ""@typeKeywords"": ""type"",
          ""@default"": ""identifier""
        }
      }],

      // whitespace
      { include: ""@whitespace"" },

      // delimiters
      [/[{}()\[\]]/, ""@brackets""],

      // operators
      [/@symbols/, {
        cases: {
          ""@operators"": ""operator"",
          ""@default"": """"
        }
      }],

      // numbers
      [/\d*\.\d+([eE][\-+]?\d+)?/, ""number.float""],
      [/\d+/, ""number""],

      // regex literal (complex rule object)
      {
        regex: /\/(?!\*)(?:[^\\/]|\\.)+\/[gimsuy]*/,
        action: { token: ""regexp"" }
      },

      // strings
      [/""/, { token: ""string.quote"", bracket: ""@open"", next: ""@string"" }],

      // template string
      [/`/, { token: ""string.quote"", bracket: ""@open"", next: ""@template"" }]
    ],

    comment: [
	  [/[^/*]+/, ""comment""],
	  [/\/\*/, ""comment"", ""@push""],
	  [/\*\//, ""comment"", ""@pop""],    
	  [/./, ""comment""]
	],

    string: [
      [/[^\\""]+/, ""string""],
      [/@escapes/, ""string.escape""],
      [/\\./, ""string.escape.invalid""],
      [/""/, { token: ""string.quote"", bracket: ""@close"", next: ""@pop"" }]
    ],

	template: [
	  [/[^\\`$]+/, ""string""],
	  [/\$\{/, { token: ""delimiter.bracket"", next: ""@braced"" }],
	  [/@escapes/, ""string.escape""],
	  [/`/, { token: ""string.quote"", bracket: ""@close"", next: ""@pop"" }]
	],

	braced: [
		[/}/, { token: ""delimiter.bracket"", next: ""@pop"" }],
		{ include: ""@root"" }
	],

    whitespace: [
      [/[ \t\r\n]+/, ""white""],
      [/\/\*/, ""comment"", ""@comment""],
      [/\/\/.*$/, ""comment""]
    ]
  }
}";

		const string MyLangConfiguration = @"{
  comments: {
    lineComment: ""//"",
    blockComment: [""/*"", ""*/""]
  },

  brackets: [
    [""{"", ""}""],
    [""["", ""]""],
    [""("", "")""]
  ],

  autoClosingPairs: [
    { open: ""{"", close: ""}"" },
    { open: ""["", close: ""]"" },
    { open: ""("", close: "")"" },
    { open: ""\"""", close: ""\"""" },
    { open: ""`"", close: ""`"" }
  ],

  surroundingPairs: [
    { open: ""{"", close: ""}"" },
    { open: ""["", close: ""]"" },
    { open: ""("", close: "")"" },
    { open: ""\"""", close: ""\"""" },
    { open: ""`"", close: ""`"" }
  ]
}";

		const string testText = @"function test(x: number) {

    /* outer comment
        /* nested comment */
    */

    let value = 10.5
    const flag = true

    let regex = /abc\d+/gi

    let str = ""hello world""

    let template = `value is ${value}`

    if (flag && value > 5) {
        return template
    }

    return null
}";

		public MainViewModel() {
			Text = @"/*
* C# Program to Display All the Prime Numbers Between 1 to 100
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VS
{
	class Program
	{
		static void Main(string[] args)
		{
			bool isPrime = true;
			Console.WriteLine(""Prime Numbers : "");
			for (int i = 2; i <= 100; i++)
			{
				for (int j = 2; j <= 100; j++)
				{
					if (i != j && i % j == 0)
					{
						isPrime = false;
						break;
					}
				}
				
				if (isPrime)
				{
					Console.Write(""\t"" +i);
				}
				isPrime = true;
			}
			Console.ReadKey();
		}
	}
}
";

			RefreshLanguagesCommand = new AsyncCommand(RefreshLanguages, CanRefreshLanguages);
		}

		public IOpenFileDialogService OpenFileDialogService { get { return GetService<IOpenFileDialogService>(); } }

		public ISaveFileDialogService SaveFileDialogService { get { return GetService<ISaveFileDialogService>(); } }

		public ICodeEditorService CodeEditorService { get { return GetService<ICodeEditorService>(); } }

		public IDialogService DialogService { get { return GetService<IDialogService>(); } }

		public string Text {
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

        public ObservableCollection<string> Languages {
            get {
                if(languages == null) {
                    languages = new ObservableCollection<string>();
                }

                return languages;
            }
        }

        public IReadOnlyList<MonacoThemeRule> Rules {
            get { return GetValue<IReadOnlyList<MonacoThemeRule>>(); }
            set { SetValue(value); }
        }

        public string? Language {
            get { return GetValue<string?>(); }
            set { SetValue(value); }
        }

        public AsyncCommand RefreshLanguagesCommand { get; private set; }

		[Command]
		public void OpenFile() {
			if(OpenFileDialogService.ShowDialog()) {
                IFileInfo file = OpenFileDialogService.Files.First();
				Text = File.ReadAllText(Path.Combine(file.DirectoryName, file.Name));
			}
		}

		public bool CanOpenFile() {
			return OpenFileDialogService != null;
		}

		[Command]
		public void SaveFile() {
			if(SaveFileDialogService.ShowDialog()) {
                IFileInfo file = SaveFileDialogService.File;
				File.WriteAllText(Path.Combine(file.DirectoryName, file.Name), Text);
				CodeEditorService.MarkAsSaved();
			}
		}

		public bool CanSaveFile() {
			return SaveFileDialogService != null;
		}

		[Command]
		public async void Initialize() {
			await RefreshLanguages();
            Language = Languages.FirstOrDefault(c => c.Contains("csharp"));
		}

		async Task RefreshLanguages() {
			if(CodeEditorService == null) {
				throw new InvalidOperationException("CodeEditorService is not available.");
			}

			Languages.Clear();
            IReadOnlyCollection<string> result = await CodeEditorService.GetLanguagesAsync();
			if(result != null) {
				Languages.AddRange(result);
			}
		}
		bool CanRefreshLanguages() {
			return CodeEditorService != null;
		}

		[Command]
		public void ChangeRules() {

            if(DialogService == null) {
                throw new InvalidOperationException("DialogService is not available.");
            }

            RulesViewModel vm = new RulesViewModel();
            if(Rules != null)
                vm.Rules.AddRange(Rules);

            UICommand buttonSave = new UICommand() {
                Id = "save",
                Caption = "Save",
                Command = new DelegateCommand(() => { vm.ApplyRulesChanges(); }),
                IsDefault = true,
                IsCancel = false
            };

            UICommand buttonCancel = new UICommand() {
                Id = "cancel",
                Caption = "Cancel",
                Command = new DelegateCommand(() => { }),
                IsDefault = false,
                IsCancel = true
            };

            UICommand result;

            result = DialogService.ShowDialog(
                dialogCommands: new UICommand[] { buttonSave, buttonCancel },
                title: "Change Rules",
                documentType: "RulesView",
                viewModel: vm
            );

            if(result != buttonSave) {
                return;
            }
            //update rules so that theme can apply it.
            Rules = new List<MonacoThemeRule>(vm.Rules);
        }

		public bool CanChangeRules() {
			return DialogService != null;
        }

        [Command]
		public async void RegisterCustomLanguage() {

			if(CodeEditorService == null) {
				throw new InvalidOperationException("CodeEditorService is not available.");
			}

			if(DialogService == null) {
				throw new InvalidOperationException("DialogService is not available.");
            }

            CustomLanguageViewModel vm = new CustomLanguageViewModel();
			vm.Monarch = MyLangMonarch;
			vm.Configuration = MyLangConfiguration;
			vm.LanguageId = "MyLang";

            UICommand buttonSave = new UICommand() {
                Id = "save",
                Caption = "Save",
                Command = new DelegateCommand(() => { }, () => { return !string.IsNullOrWhiteSpace(vm.LanguageId); }),
			    IsDefault = true,
                IsCancel = false
            };

            UICommand buttonCancel = new UICommand() {
                Id = "cancel",
                Caption = "Cancel",
                Command = new DelegateCommand(() => { }),
                IsDefault = false,
                IsCancel = true
            };

            UICommand result;

            result = DialogService.ShowDialog(
                dialogCommands: new UICommand[] { buttonSave, buttonCancel },
                title: "Register Custom Language",
				documentType: "CustomLanguageView",
                viewModel: vm
            );

            if(result != buttonSave) {
				return;
            }

            LanguageDescriptor myLang = new LanguageDescriptor {
				Id = vm.LanguageId,
				Monarch = vm.Monarch,
				Configuration = vm.Configuration
			};

			CodeEditorService.RegisterLanguage(myLang);
			await RefreshLanguages();
			Language = vm.LanguageId;

            Text = testText;
		}

		public bool CanRegisterCustomLanguage() {
			return DialogService != null && CodeEditorService != null;
        }
	}
}
