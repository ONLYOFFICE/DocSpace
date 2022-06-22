import React from "react";

const SubInfoPanelHeader = ({ children }) => {
  const content = children?.props?.children;
  return <>{content}</>;
};

SubInfoPanelHeader.displayName = "SubInfoPanelHeader";

export default SubInfoPanelHeader;
