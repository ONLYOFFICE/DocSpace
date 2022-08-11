import React, { useState, useEffect } from "react";
import MobileView from "./MobileView";
import DesktopView from "./DesktopView";
import { withRouter } from "react-router-dom";
import { size } from "@docspace/components/utils/device";

const Backup = (props) => {
  const [isMobileView, setIsMobileView] = useState(false);

  useEffect(() => {
    checkWidth();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  const checkWidth = () => {
    window.innerWidth <= size.smallTablet
      ? setIsMobileView(true)
      : setIsMobileView(false);
  };

  return isMobileView ? <MobileView {...props} /> : <DesktopView {...props} />;
};

export default withRouter(Backup);
