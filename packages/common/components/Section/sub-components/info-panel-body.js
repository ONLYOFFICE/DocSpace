import Scrollbar from "@docspace/components/scrollbar";
import React from "react";

const SubInfoPanelBody = ({ children }) => {
  const content = children?.props?.children;

  return (
    <Scrollbar scrollclass="section-scroll" stype="mediumBlack">
      {content}
    </Scrollbar>
  );
};

SubInfoPanelBody.displayName = "SubInfoPanelBody";

export default SubInfoPanelBody;
