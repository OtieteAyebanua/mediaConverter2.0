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
            document.getElementById('file-name').textContent = file.name;
            document.getElementById('file-size').textContent = (file.size / (1024 * 1024)).toFixed(2) + " MB";
            
            // Set Original Preview
            document.getElementById('originalVideo').src = URL.createObjectURL(file);
            
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
            const url = `${window.location.protocol}//${window.location.host}/api/video/convert`;
            const response = await fetch(url, {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                const blob = await response.blob();
                const objectUrl = URL.createObjectURL(blob);

                // Try to get filename from header
                let filename = 'converted-video';
                const disposition = response.headers.get('content-disposition');
                if (disposition) {
                    const match = /filename=\"?([^\";]+)\"?/.exec(disposition);
                    if (match && match[1]) filename = match[1];
                } else {
                    // Fallback to selected extension
                    const fd = new FormData(convertForm);
                    const ext = fd.get('TargetExtension');
                    if (ext) filename = `converted-video${ext}`;
                }

                document.getElementById('convertedVideo').src = objectUrl;
                document.getElementById('downloadBtn').href = objectUrl;
                document.getElementById('downloadBtn').download = filename;

                processingState.classList.add('hidden');
                resultModal.classList.remove('hidden');
            } else {
                throw new Error();
            }
        } catch (err) {
            alert("Error converting video. The format or codec might be unsupported.");
            location.reload();
        }
    });
});