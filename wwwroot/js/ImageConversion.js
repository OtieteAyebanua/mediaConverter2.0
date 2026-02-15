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
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        try {
            const response = await fetch('', {
                method: 'POST',
                body: formData,
                headers: { 'RequestVerificationToken': token }
            });

            if (response.ok) {
                const data = await response.json();
                document.getElementById('convertedImg').src = data.convertedUrl;
                document.getElementById('downloadBtn').href = data.convertedUrl;
                document.getElementById('downloadBtn').download = `converted-image.${data.extension}`;
                
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