import React from "react";
import styled from "styled-components";
import { Headline } from "asc-web-common";

const StyledHeadline = styled(Headline)`
  margin-top: -1px;
`;
const SectionHeaderContent = ({ setting, t }) => {
  const title = setting[0].toUpperCase() + setting.slice(1);
  return (
    <StyledHeadline className="headline-header" type="content" truncate={true}>
      {t(`${title}`)}
    </StyledHeadline>
  );
};

export default SectionHeaderContent;
