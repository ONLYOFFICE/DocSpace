import React, { useState, useEffect } from "react";
import PortalDeactivationSection from "./portalDeactivation";
import PortalDeletionSection from "./portalDeletion";
import { DeleteDataLayout } from "./StyledDeleteData";
import MobileView from "./mobileView";
import { size } from "@appserver/components/utils/device";

const DeleteData = () => {
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

  if (isMobileView) return <MobileView />;
  return (
    <DeleteDataLayout>
      <PortalDeactivationSection />
      <hr />
      <PortalDeletionSection />
    </DeleteDataLayout>
  );
};

export default DeleteData;
