# EPFArchive
EPF Archive (East Point File System developed by East Point Software) Manager, capable of packing/unpacking various DOS games content.

More information about EPF File format and games it was used in can be found on following site:
http://www.shikadi.net/moddingwiki/EPF_Format

## Is this working? 
**Yeah!**

## What is ready?

### EPFArchive class functionality
- Opening archives
- Creating new archives
- Reading archive entries data using streams
- Updating archive entries data using streams
- Adding new archive entries
- Closing archives with saving changes
- Adding/updating/removing hidden data in archive

### EPF Archive Manager functionality
- WinForms or WPF are used as UI front-end
- Creating new EPF archives for saving
- Opening EPF archives in read-only mode, extracting selected or all entries
- Opening EPF archives in read-write mode, extracting, adding, removing entries
- Showing compression ratio on each entry
- Adding/updating/removing hidden data in archive

## What is in current development? 
**Nothing**

## What could be changed/improved? 
- Add better looking icons in UI
- Improving packing/unpacking performance


## Development stuff
### Source code language
**C#**

### Platform
**.NET 4.6.1/4.7.2** for now

### DevEnv
**MSVS 2017** (Community or compatible)

### Design architecture
**MVVM(Model-View-Viewmodel)

### Libraries
* **WinForms/WPF** - used for implementing views for EPF Archive Manager UI



