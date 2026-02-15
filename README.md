# mediaConverter

Lightweight ASP.NET Core web app for image and video compression/conversion.

## Overview

This project provides web UI pages and API endpoints to compress and convert images and videos using ImageSharp and FFmpeg (via FFMpegCore). It includes:

- Image compression and conversion
- Video compression and conversion
- Razor Pages UI and JavaScript front-end
- Controller APIs for programmatic access

## Requirements

- .NET 8 SDK
- FFmpeg installed and available on PATH (brew install ffmpeg on macOS)

Optional libraries are managed via NuGet (ImageSharp, FFMpegCore).

## Getting Started

1. Restore and build:

```bash
dotnet restore
dotnet build
```

2. Run the app:

```bash
dotnet run
```

3. Open the site in your browser (default shown by `dotnet run`) and navigate to the pages under `/ImageCompression`, `/ImageConversion`, `/VideoCompression`, and `/VideoConversion`.

## Important Pages

- Image Compress: `/ImageCompression/CompressImage`
- Image Convert: `/ImageConversion/ImageConversion`
- Video Compress: `/VideoCompression/CompressVideo`
- Video Convert: `/VideoConversion/VideoConversion`

These pages are Razor Pages under `Pages/` and include corresponding JS in `wwwroot/js/`.

## API Endpoints

Image API (controller: `ImageController`)
- POST `/api/image/compress` — form field: `file` (IFormFile). Returns compressed image as binary `FileContentResult`.
- POST `/api/image/convert` — form fields: `ImageFile` (IFormFile), `TargetFormat` (string; e.g. `jpg`, `png`, `webp`, `gif`, `bmp`, `tiff`). Returns converted image as binary `FileContentResult`.

Video API (controller: `VideoController`)
- POST `/api/video/compress` — form fields: `VideoFile` (IFormFile), `CrfValue` (int). Returns compressed video as binary `FileContentResult`.
- POST `/api/video/convert` — form fields: `VideoFile` (IFormFile), `TargetExtension` (string; e.g. `.mp4`, `.mkv`, `.mov`), `CrfValue` (int). Returns converted video as binary `FileContentResult`.

Notes:
- API controllers have `[IgnoreAntiforgeryToken]` applied because requests are sent via JS fetch and authenticated APIs are expected to use different protections.
- The front-end JS expects binary responses (blobs) and creates object URLs for preview and downloads.

## File Structure (key files)

- `Program.cs` — app startup, routing
- `Controllers/ImageController.cs`, `Controllers/VideoController.cs` — API controllers
- `Services/imageComression/ImageService.cs` — image compress/convert logic (ImageSharp)
- `Services/videoService/VideoService.cs` — video compress/convert logic (FFMpegCore)
- `Services/coordinators/Coordinator.cs` — routes file operations to the right service method
- `Pages/*` — Razor Pages UI
- `wwwroot/js/*` — front-end JavaScript

## Developer Notes

- Ensure `ffmpeg` is installed and accessible by the runtime — FFMpegCore calls the local ffmpeg/ffprobe binaries.
- The `Coordinator` currently throws for unsupported formats; controllers catch and return `400` for those.
- Several controller methods bind form fields explicitly (e.g. `[FromForm(Name = "VideoFile")]`) to match the UI form names.
- The JS files use absolute URLs constructed from `window.location` to avoid Razor Pages route collisions.

## Known Warnings

- Build shows nullability warnings in `Services/coordinators/Coordinator.cs`. These are non-blocking but should be addressed by adding null checks or enabling nullable annotations.

## Testing

- Use the web UI to test a conversion/compression flow.
- For API tests, use curl or HTTP clients:

```bash
curl -X POST "http://localhost:5000/api/image/compress" -F "file=@/path/to/image.png" --output compressed.png
```

## Contributing

Fork the repo, create a branch, and submit PRs. Add tests when modifying encoding logic.

## Contact

If you want changes to the README (more examples, architecture diagrams, CI), tell me what to add.
