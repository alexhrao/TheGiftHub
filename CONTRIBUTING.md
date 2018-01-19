# Contributing to The Gift Hub

Before I say anything else, let me tell you how glad I am that you've decided to contribute!

Below, I've listed some basic guidelines for contributing to this project. These are *guidelines* - they are **not** rules.
If in doubt, use your best judgement or email me!

#### Table of Contents

[Code of Conduct](#code-of-conduct)

[Contribution Mechanisms](#contribution-mechanisms)
 * [General Method](#general-method)

[Coding Style](#code-style)
 * [C# Code Style](#c-code-style)
 * [Documentation](#documentation)
 * [Git Commits](#git-commits)
 * [Pull Requests](#pull-requests)
 
## Code of Conduct

Obviously, contribution and interaction with this project requires that you adhere to our [Code of Conduct](CODE_OF_CONDUCT.md).

## Contribution Mechanisms

We primarily use *pull requests* to facilitate development. At least, that's the goal. Right now, development is done on the *Development*
branch, but once we reach our first release, we'll move to pull requests.

### General Method

In general, the method is as follows:
1. Fork the [Repository](https://github.com/alexhrao/TheGiftHub.git)
2. Make your coding changes (ensuring you follow the [Coding Style](#coding-style)!)
3. Create a pull request to merge into *Development* - Do **not** try to merge with Testing or Production!
**Be sure to follow the [pull request documentation standards](#pull-requests)**
4. If it's denied, fix any problems raised and resubmit
5. If it's accepted, celebrate!

## Coding Style

Here at The Gift Hub, we pride ourselves on ensuring maximum readability, since, as everyone knows, readability is paramount to maintainability!
As such, we have standards for how code should look, and how it should be documented.

### Git Commit Messages

* Commit messages should be in the imperative case - Add Functionality, not Added functionality
* Descriptions are not required unless the documentation for a commit is more than 50 characters
* Descriptions **are** required if it's a release version, as well as a tag
* Because we value readability, commits that just fix grammar or refactor code are welcomed!
* Branches must also be in PascalCase: AddContribution, not addContribution or add-contribution

### Documentation

Use the standard Microsoft XML approach:

```csharp
/// <summary>
/// Hello World! I'm a short summary (not more than one line)
/// </summary>
/// <remarks>
/// Well howdy there! I'm here to provide more information than my summary, 
/// and any caveats and/or worthwhile examples.
/// I can span as many or as little lines as needed, and I'm not required (though encouraged)
/// </remarks>
```

Note that actual documentation is created on build by DocFX

### C# Code Style

A couple of notes here:
 
* Always make a new line for your curly braces (Visual Studio does this automatically)
* **Always** provide documentation for any new public feature - follow these [guidelines](#documentation)
* Except in rare circumstances, no line shall be longer than 100 characters
 
In addition to these guidelines, we also have naming rules:
* If a property or method is |public|, it **must**:
  * Have XML documentation
  * Be in PascalCase
* If the file is an |interface|, it's file name must start with "I"
* All file names are also PascalCase
* Acronyms (URL, HTML, CSS, etc.) are in PascalCase when used in the name of a method or property. For example:
  * XmlDocument, HtmlEntity, CssPrettyPrint, HtmlMarker, etc... **not** XMLDocument, HTMLEntity, etc.
* Properties are always listed before methods
* SQL Queries are indented properly within the cs file - not just one long string

### Pull Requests

A pull request can be simple, but it must meet the following criteria:

* It has a short, clear title
* It has a description that has been styled using Markdown
* If it references issues, they must be formally referenced using the # convention
* User mentions must use the @ interface (i.e., @alexhrao)

Again, thank you for your contribution, and the team looks forward to working with you!

Best Regards,

The Gift Hub
