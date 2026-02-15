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

        const file = fileElem.files[0];
        const targetFormat = document.querySelector('select[name="TargetExtension"]').value;
        
        const formData = new FormData();
        formData.append('file', file);

        try {
            const response = await fetch(`/api/video/convert?targetFormat=${targetFormat}`, {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                const convertedBlob = await response.blob();
                const convertedUrl = URL.createObjectURL(convertedBlob);
                document.getElementById('convertedVideo').src = convertedUrl;
                const downloadBtn = document.getElementById('downloadBtn');
                downloadBtn.href = convertedUrl;
                downloadBtn.download = `converted-video${targetFormat}`;
                
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