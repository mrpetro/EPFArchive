# EPFArchive
EPF Archive (East Point File System developed by East Point Software) Manager, capable of packing/unpacking various couple of DOS games content

More information about EPF File format and games it was used in can be found on following site:
http://www.shikadi.net/moddingwiki/EPF_Format

**Current EPF Archive class functionality:**
- Opening archives
- Reading archive entries data using streams
- Updating archive entries data using streams
- Closing archives with saving changes

**Current EPF Archive UI functionality:**
- Opening EPF archives in read-only mode and extracting selected entries

**EPF Archive class functionality to add/improve:**
- [Feature] Creating new archive entries
- [Feature] Adding hidden data to archive
- [Improvement] LZW compression algorithm used for storing entries is slooooooooow. This has to improve.

**EPF Archive UI functionality to add/improve:**
- [Feature]Opening EPF archive in read/write mode. This implies adding new entries, modifying/removing existing entries 
