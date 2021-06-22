importScripts("https://www.gstatic.com/firebasejs/8.6.7/firebase-app.js");
importScripts("https://www.gstatic.com/firebasejs/8.6.7/firebase-messging.js");

var firebaseConfig = {
  apiKey: "AIzaSyD89-RSZMDJb99M6boo7om5qOR7ZXGHGYA",
  authDomain: "as-messagebar.firebaseapp.com",
  projectId: "as-messagebar",
  storageBucket: "as-messagebar.appspot.com",
  messagingSenderId: "624985220555",
  appId: "1:624985220555:web:825fdc1df2d19df157424e",
  measurementId: "G-PJDDTX8025",
};
// Initialize Firebase
firebase.initializeApp(firebaseConfig);

const messaging = firebase.messaging();

messaging
  .getToken({
    vapidKey:
      "dBQoC95KCChN4jv_JT3qjj:APA91bG1lC05XNP2Nj-YiVgJ8HvuiXtVTkoqMlTDZJiI6xjXFPHbL968-xeZ9bxwJCO13K7_gWhYcYVU7fgaq_xYr-PO7xGtBUFPqlLZ2kawKr543dR67NKNlmkXmn4A8tqbod4neBPH",
  })
  .then((currentToken) => {
    if (currentToken) {
      // Send the token to your server and update the UI if necessary
      // ...
    } else {
      // Show permission request UI
      console.log(
        "No registration token available. Request permission to generate one."
      );
      // ...
    }
  })
  .catch((err) => {
    console.log("An error occurred while retrieving token. ", err);
    // ...
  });
