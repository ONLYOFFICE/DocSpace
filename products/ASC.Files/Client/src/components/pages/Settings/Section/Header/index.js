import React from "react";
import { Headline } from "asc-web-common";

const SectionHeaderContent = ({ title }) => {
  return (
    <Headline className="headline-header" type="content" truncate={true}>
      {title}
    </Headline>
  );
};

export default SectionHeaderContent;
