# Prior Stimulation Feedback to Improve EMG Reaction Times

We investigated whether prior stimulation using visual or tactile cues at four different target muscles (biceps, triceps, upper leg, calf) can help reduce the time to perform isometric muscle contractions in a response-based VR experiment (N=21). 
The results show that prior stimulation decreases EMG reaction times with visual, vibrotactile, and electrotactile cues. 

Unity Project is created using the Biosignalplux Unity sample in Unity 2019.
It was then migrated to 2021.3.5f1 and integrated the signal processing from biosignals to the biosignalplux EMG Sensor (1) with the 4-Channel Hub from biosignalsplux.
The project is implemented to work with a HTC Vive Pro Eye, Version 2022.

Unity Project and Arduino code for EMG multi-modal prior stimulation in Virtual Reality. 
Intended to be used for visual, vibrotactile (2), and electrotactile (3) prior stimulation in an EMG-based reaction test experiment with isometric EMG input.

To motivate further investigations on the base of this research we provide the dataset including the Unity project and the log files. A detailed description of the study procedure and hardware integration can be found in the related dissertation (*doctoral thesis reference here*).

![conditionsPriorStim](https://github.com/user-attachments/assets/7e59d7f4-d2c0-483f-801f-84df284f256a)

```@inproceedings{SehrtCHI2024,
author = {Sehrt, Jessica and Leite Ferreira, Leonardo and Weyers, Karsten and Mahmood, Amir and Kosch, Thomas and Schwind, Valentin},
title = {Improving Electromyographic Muscle Response Times through Visual and Tactile Prior Stimulation in Virtual Reality},
year = {2024},
isbn = {978-1-4503-9421-5},
publisher = {Association for Computing Machinery},
address = {New York, NY, USA},
url = {https://doi.org/10.1145/3613904.3642091},
doi = {[10.1145/3544548.3580738](https://doi.org/10.1145/3613904.3642091)},
keywords = {Physiological Sensing, Electromyography, Electrical Muscle Stimulation, Virtual Reality, Assistive Systems},
location = {Honolulu, USA},
series = {CHI '24}
}
