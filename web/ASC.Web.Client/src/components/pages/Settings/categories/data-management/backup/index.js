import React, { useState } from "react";
import MobileView from "./MobileView";
import DesktopView from "./DesktopView";
import { isMobileOnly } from "react-device-detect";
import { withRouter } from "react-router-dom";

const Backup = (props) => {
  return isMobileOnly ? <MobileView {...props} /> : <DesktopView {...props} />;
};

export default withRouter(Backup);
