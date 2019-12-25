import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { utils, Icons, Text } from "asc-web-components";
const { tablet, mobile } = utils.device;

const StyledArticlePinPanel = styled.div`
  border-top: 1px solid #eceef1;
  height: 56px;
  min-height: 56px;
  display: none;

  @media ${tablet} {
    display: block;
  }

  @media ${mobile} {
    display: none;
  }

  div {
    display: flex;
    align-items: center;
    cursor: pointer;
    user-select: none;
    height: 100%;

    span {
      margin-left: 8px;
    }
  }
`;

const ArticlePinPanel = React.memo(props => {
  //console.log("PageLayout ArticlePinPanel render");
  const { pinned, pinText, onPin, unpinText, onUnpin } = props;

  return (
    <StyledArticlePinPanel>
      {pinned ? (
        <div onClick={onUnpin}>
          <Icons.CatalogUnpinIcon size="medium" />
          <Text as='span' color="#555F65">{unpinText}</Text>
        </div>
      ) : (
        <div onClick={onPin}>
          <Icons.CatalogPinIcon size="medium" />
          <Text as='span' color="#555F65">{pinText}</Text>
        </div>
      )}
    </StyledArticlePinPanel>
  );
});

ArticlePinPanel.displayName = "ArticlePinPanel";

ArticlePinPanel.propTypes = {
  pinned: PropTypes.bool,
  pinText: PropTypes.string,
  onPin: PropTypes.func,
  unpinText: PropTypes.string,
  onUnpin: PropTypes.func
};

export default ArticlePinPanel;
