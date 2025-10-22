# Changelog

All notable changes to Adaptive NPC will be documented in this file.

## [1.0.1] - 2024-01-XX

### Fixed
- OpenAIProvider constructor now properly accepts personality parameter
- HybridProvider constructor signature matches usage
- Added missing `using System.Threading.Tasks` statements
- Proper error handling in all async methods
- Unity WebRequest memory leak with proper disposal
- package.json Unity version correctly specified (2021.3.0f1)

### Added
- Comprehensive error handling with try-catch blocks
- Null safety checks throughout codebase
- Quick Test menu item (AdaptiveNPC → Quick Test)
- WebRequest proper resource disposal

### Changed
- Improved error messages with component name context
- Better fallback behavior when AI fails

## [1.0.0] - 2024-01-XX

### Added
- Initial release
- Core CognitiveCompanion component
- Memory system with persistence
- Pattern recognition system
- Template-based response system
- OpenAI integration (optional)
- PlayerPrefs save provider
- Comprehensive documentation
- Unit tests
- Demo scenes
