import React from "react";
import { withTranslation } from "react-i18next";

const SeveralItems = () => {
  return (
    <div className="no-thumbnail-img-wrapper">
      <img
        size="96px"
        className="no-thumbnail-img"
        src="images/empty_screen.png"
      />
    </div>
  );
};

export default withTranslation(["InfoPanel"])(SeveralItems);
