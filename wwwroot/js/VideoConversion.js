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
                const data = await response.json();
                document.getElementById('convertedVideo').src = data.convertedUrl;
                document.getElementById('downloadBtn').href = data.convertedUrl;
                document.getElementById('downloadBtn').download = `converted-video${data.extension}`;
                
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