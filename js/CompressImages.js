const dropArea = document.getElementById("drop-area");
const fileElem = document.getElementById("fileElem");
const fileInfo = document.getElementById("file-info");
const submitBtn = document.getElementById("submitBtn");

let selectedFile = null;
let compressedBlob = null;

dropArea.onclick = () => fileElem.click();

["dragenter", "dragover"].forEach(e =>
  dropArea.addEventListener(e, (event) => {
    event.preventDefault();
    dropArea.classList.add("highlight");
  })
);

["dragleave", "drop"].forEach(e =>
  dropArea.addEventListener(e, (event) => {
    event.preventDefault();
    dropArea.classList.remove("highlight");
  })
);

dropArea.addEventListener("drop", (event) => {
  event.preventDefault();
  handleFiles(event.dataTransfer.files);
});

fileElem.onchange = (event) => handleFiles(event.target.files);

function handleFiles(files) {
  if (files.length === 0) return;

  selectedFile = files[0];
  document.getElementById("file-name").textContent = selectedFile.name;
  document.getElementById("file-size").textContent =
    (selectedFile.size / 1024).toFixed(2) + " KB";
  fileInfo.classList.remove("hidden");

  const reader = new FileReader();
  reader.onload = (e) => document.getElementById("originalPreview").src = e.target.result;
  reader.readAsDataURL(selectedFile);
}

submitBtn.onclick = async () => {
  if (!selectedFile) {
    alert("Please select an image first.");
    return;
  }

  submitBtn.disabled = true;
  submitBtn.textContent = "Optimizing...";

  const formData = new FormData();
  formData.append("file", selectedFile);

  try {
    const response = await fetch("/api/image/compress", {
      method: "POST",
      body: formData
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || "Server error during compression.");
    }

    compressedBlob = await response.blob();
    const compressedUrl = URL.createObjectURL(compressedBlob);

    document.getElementById("compressedPreview").src = compressedUrl;

    const origSizeKB = selectedFile.size / 1024;
    const compSizeKB = compressedBlob.size / 1024;

    document.getElementById("origSizeDisplay").textContent = origSizeKB.toFixed(2) + " KB";
    document.getElementById("compSizeDisplay").textContent = compSizeKB.toFixed(2) + " KB";

    const savings = ((origSizeKB - compSizeKB) / origSizeKB) * 100;
    const badge = document.getElementById("savingsBadge");
    badge.textContent = `-${savings.toFixed(0)}% SMALLER`;
    badge.classList.remove("hidden");

    const downloadBtn = document.getElementById("downloadBtn");
    downloadBtn.href = compressedUrl;
    downloadBtn.download = `compressed_${selectedFile.name}`;

    showModal();

  } catch (err) {
    alert("Compression failed: " + err.message);
  } finally {
    submitBtn.disabled = false;
    submitBtn.textContent = "Compress Now";
  }
};

function showModal() {
  const modal = document.getElementById("resultModal");
  const content = document.getElementById("modalContent");

  modal.classList.remove("hidden");
  setTimeout(() => {
    content.style.transform = "scale(1) translateY(0)";
    content.style.opacity = "1";
    modal.style.opacity = "1";
  }, 10);
}

function closeModal() {
  const modal = document.getElementById("resultModal");
  const content = document.getElementById("modalContent");

  content.style.transform = "scale(0.9) translateY(2rem)";
  content.style.opacity = "0";
  modal.style.opacity = "0";

  setTimeout(() => {
    modal.classList.add("hidden");
    fileInfo.classList.add("hidden");
    document.getElementById("originalPreview").src = "";
    document.getElementById("compressedPreview").src = "";

    if (compressedBlob) {
      URL.revokeObjectURL(compressedBlob);
      compressedBlob = null;
    }

    selectedFile = null;
  }, 300);
}
