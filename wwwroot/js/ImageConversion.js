document.addEventListener('DOMContentLoaded', () => {
    const fileElem = document.getElementById('fileElem');
    const dropArea = document.getElementById('drop-area');
    const fileInfo = document.getElementById('file-info');
    const convertForm = document.getElementById('convertForm');
    const processingState = document.getElementById('processing-state');
    const resultModal = document.getElementById('resultModal');

    dropArea.addEventListener('click', () => fileElem.click());

    fileElem.addEventListener('change', function() {
        if (this.files && this.files[0]) {
            const file = this.files[0];
            const reader = new FileReader();
            
            reader.onload = (e) => {
                document.getElementById('image-thumb').src = e.target.result;
                document.getElementById('originalImg').src = e.target.result;
            };
            reader.readAsDataURL(file);

            document.getElementById('file-name').textContent = file.name;
            document.getElementById('file-size').textContent = (file.size / (1024 * 1024)).toFixed(2) + " MB";
            
            dropArea.classList.add('hidden');
            fileInfo.classList.remove('hidden');
        }
    });

    convertForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        fileInfo.classList.add('hidden');
        processingState.classList.remove('hidden');

        const formData = new FormData(convertForm);

        try {
            const url = `${window.location.protocol}//${window.location.host}/api/image/convert`;
            const response = await fetch(url, {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                    const blob = await response.blob();
                    const objectUrl = URL.createObjectURL(blob);

                    // Try to extract filename from Content-Disposition header
                    let filename = 'converted-image';
                    const disposition = response.headers.get('content-disposition');
                    if (disposition) {
                        const match = /filename=\"?([^\";]+)\"?/.exec(disposition);
                        if (match && match[1]) filename = match[1];
                    } else {
                        // Fallback: use selected target format extension
                        const form = new FormData(convertForm);
                        const target = form.get('TargetFormat');
                        if (target) filename = `converted-image.${target.toString().replace('.', '')}`;
                    }

                    document.getElementById('convertedImg').src = objectUrl;
                    document.getElementById('downloadBtn').href = objectUrl;
                    document.getElementById('downloadBtn').download = filename;

                    processingState.classList.add('hidden');
                    resultModal.classList.remove('hidden');
                } else {
                    throw new Error();
                }
        } catch (err) {
            alert("Error converting image.");
            location.reload();
        }
    });
});