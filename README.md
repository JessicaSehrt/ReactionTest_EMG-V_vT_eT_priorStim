# Prior Stimulation Feedback to Improve EMG Reaction Times

This research presents results that indicate that prior stimulation decreases EMG reaction times with visual, vibrotactile, and electrotactile cues and the fastest response by the calf muscle. 

Unity Project is created using the Biosignalplux Unity sample in Unity 2019. It was then migrated to 2021.3.5f1 and integrated the signal processing from biosignals to the biosignalplux EMG Sensor (1) with the 4-Channel Hub from biosignalsplux. The project is implemented to work with a HTC Vive Pro Eye, Version 2022. Unity Project and Arduino code for EMG multi-modal prior stimulation in Virtual Reality. 
Intended to be used for visual, vibrotactile (2), and electrotactile (3) prior stimulation in an EMG-based reaction test experiment with isometric EMG input. It also integrates the output to 2 SEM 47 EMS/TENS devices with 2 Arduino UNO R3 connected to four solid-state relays (Vishay LH1546ADF optocoupler) acting like switches of the four TENS channels, as well as four coin-type vibration motors (Iduino TC-
9520268).

To motivate further investigations on the base of this research we provide the dataset including the Unity project, the log files from the studies, and the scripts we used for the evaluation. 
A detailed description of the study procedure and hardware integration can be found in the related dissertation (*doctoral thesis reference here, coming soon*).

You find the whole evaluation data for the muscle reaction study here: https://www.dropbox.com/scl/fo/5wablb0jim8c85ifuk75n/AOhCXAQQkBtRjeaiL_F1bnM?rlkey=t2n3dhrgu9ublqeo1cc9wnqgu&st=swjn6rkm&dl=0

We investigated whether prior stimulation using visual or tactile cues at four different target muscles (biceps, triceps, upper leg, calf) can help reduce the time to perform isometric muscle contractions in a response-based VR experiment (N=21). 

![conditionsPriorStim](https://github.com/user-attachments/assets/98de6338-5edb-496c-bfa0-7993ef670dee)

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
