# EPFArchive
EPF Archive (East Point File System developed by East Point Software) Manager, capable of packing/unpacking various DOS games content.

More information about EPF File format and games it was used in can be found on following site:
http://www.shikadi.net/moddingwiki/EPF_Format

**Current EPF Archive class functionality:**
- Opening archives
- Reading archive entries data using streams
- Updating archive entries data using streams
- Adding new archive entries
- Closing archives with saving changes

**Current EPF Archive UI functionality:**
- WinForms are used as UI front-end
- Opening EPF archives in read-only mode, extracting selected or all entries
- Opening EPF archives in read-write mode, extracting, adding, removing entries
- Showing compression ratio on each entry

**EPF Archive class functionality to add/improve:**
- [Feature] Adding hidden data to archive
- [Feature] Add SaveAs method to EPF Archive

**EPF Archive UI functionality to add/improve:**
- [Feature] Implement Save As... functionality
- [Feature] Add better icons
- [Feature] Add WPF as a optional UI front-end
