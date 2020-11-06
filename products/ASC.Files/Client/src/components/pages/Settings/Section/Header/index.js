import React from "react";
import { Headline } from "asc-web-common";

const SectionHeaderContent = ({ setting, t }) => {
  const title = setting[0].toUpperCase() + setting.slice(1);
  return (
    <Headline className="headline-header" type="content" truncate={true}>
      {t(`${title}Settings`)}
    </Headline>
  );
};

export default SectionHeaderContent;
