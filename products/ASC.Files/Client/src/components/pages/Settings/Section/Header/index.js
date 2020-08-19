import React from 'react';
import styled from "styled-components";
import { Headline } from 'asc-web-common';
import { utils } from "asc-web-components";
import { withTranslation } from "react-i18next";

const { tablet, desktop } = utils.device;

const StyledContainer = styled.div`

`;

const SectionHeaderContent = ({title}) => {
  return (
    <Headline 
      className='headline-header' 
      type="content" 
      truncate={true}>
        {title}
    </Headline>
  );
}

export default SectionHeaderContent;