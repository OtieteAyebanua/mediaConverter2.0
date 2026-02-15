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
    const token = document.querySelector(
      'input[name="__RequestVerificationToken"]'
    ).value;

    try {
      const response = await fetch("", {
        method: "POST",
        body: formData,
        headers: { RequestVerificationToken: token },
      });

      if (response.ok) {
        const data = await response.json();

        // Populate Modal
        document.getElementById("compressedVideo").src = data.compressedUrl;
        document.getElementById("downloadBtn").href = data.compressedUrl;
        document.getElementById("compSizeDisplay").textContent =
          data.compressedSize;

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
