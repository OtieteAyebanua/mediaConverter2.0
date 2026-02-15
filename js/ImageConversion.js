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

        const file = fileElem.files[0];
        const targetFormat = document.querySelector('select[name="TargetFormat"]').value;
        
        const formData = new FormData();
        formData.append('file', file);

        try {
            const response = await fetch(`/api/image/convert?targetFormat=${targetFormat}`, {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                const convertedBlob = await response.blob();
                const convertedUrl = URL.createObjectURL(convertedBlob);
                document.getElementById('convertedImg').src = convertedUrl;
                const downloadBtn = document.getElementById('downloadBtn');
                downloadBtn.href = convertedUrl;
                downloadBtn.download = `converted-image.${targetFormat}`;
                
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