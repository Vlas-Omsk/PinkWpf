﻿using System.Windows.Markup;
using System.Windows;

[assembly: XmlnsDefinition(@"http://schemas.microsoft.com/winfx/2006/xaml/presentation", "PinkWpf.Controls")]
[assembly: XmlnsDefinition(@"http://schemas.microsoft.com/winfx/2006/xaml/presentation", "PinkWpf.MarkupExtensions")]
[assembly: XmlnsDefinition(@"http://schemas.microsoft.com/winfx/2006/xaml/presentation", "PinkWpf.MarkupExtensions.AttachedProperties")]
[assembly: XmlnsDefinition(@"http://schemas.microsoft.com/winfx/2006/xaml/presentation", "PinkWpf.MarkupExtensions.Converters")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]