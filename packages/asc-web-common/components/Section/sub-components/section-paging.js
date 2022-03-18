import React from "react";
import styled from "styled-components";
import equal from "fast-deep-equal/react";

const StyledSectionPaging = styled.div`
  margin: 16px 0 0;
`;

class SectionPaging extends React.Component {
  render() {
    return <StyledSectionPaging {...this.props} />;
  }
}

SectionPaging.displayName = "SectionPaging";

export default SectionPaging;
