document.addEventListener("DOMContentLoaded", () => {
  const fileElem = document.getElementById("fileElem");
  const dropArea = document.getElementById("drop-area");
  const fileInfo = document.getElementById("file-info");
  const uploadForm = document.getElementById("uploadForm");
  const processingState = document.getElementById("processing-state");
  const resultModal = document.getElementById("resultModal");

  // Handle Drop Area click
  dropArea.addEventListener("click", () => fileElem.click());

  // File selection event
  fileElem.addEventListener("change", function () {
    if (this.files && this.files[0]) {
      const file = this.files[0];
      document.getElementById("file-name").textContent = file.name;
      document.getElementById("file-size").textContent =
        (file.size / (1024 * 1024)).toFixed(2) + " MB";

      // Preview Original
      document.getElementById("originalVideo").src = URL.createObjectURL(file);

      dropArea.classList.add("hidden");
      fileInfo.classList.remove("hidden");
    }
  });

  // Submit via AJAX
  uploadForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    fileInfo.classList.add("hidden");
    processingState.classList.remove("hidden");

    const formData = new FormData(uploadForm);

    try {
      const url = `${window.location.protocol}//${window.location.host}/api/video/compress`;
      const response = await fetch(url, {
        method: "POST",
        body: formData
      });

      if (response.ok) {
        const blob = await response.blob();
        const objectUrl = URL.createObjectURL(blob);

        // Set compressed preview and download
        document.getElementById("compressedVideo").src = objectUrl;
        document.getElementById("downloadBtn").href = objectUrl;

        // Try get filename from header
        let filename = 'compressed_video';
        const disposition = response.headers.get('content-disposition');
        if (disposition) {
          const match = /filename=\"?([^\";]+)\"?/.exec(disposition);
          if (match && match[1]) filename = match[1];
        }
        document.getElementById("downloadBtn").download = filename;

        // Display size
        const compSizeKB = (blob.size / 1024).toFixed(2) + ' KB';
        document.getElementById("compSizeDisplay").textContent = compSizeKB;

        processingState.classList.add("hidden");
        resultModal.classList.remove("hidden");
      } else {
        throw new Error("Compression failed");
      }
    } catch (err) {
      alert("Error processing video. Try a smaller file.");
      location.reload();
    }
  });

  window.closeModal = () => location.reload();
});
