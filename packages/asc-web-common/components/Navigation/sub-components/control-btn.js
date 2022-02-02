import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import ContextMenuButton from "@appserver/components/context-menu-button";

const StyledContainer = styled.div`
  margin-left: 4px;
  display: flex;
  align-items: center;
`;

const ControlButtons = ({
  personal,
  isRootFolder,
  canCreate,
  getContextOptionsFolder,
  getContextOptionsPlus,
}) => {
  return !isRootFolder && canCreate ? (
    <StyledContainer>
      <ContextMenuButton
        className="add-button"
        directionX="right"
        iconName="images/plus.svg"
        size={17}
        color="#A3A9AE"
        hoverColor="#657077"
        isFill
        getData={getContextOptionsPlus}
        isDisabled={false}
      />
      {!personal && (
        <ContextMenuButton
          className="option-button"
          directionX="right"
          iconName="images/vertical-dots.react.svg"
          size={17}
          color="#A3A9AE"
          hoverColor="#657077"
          isFill
          getData={getContextOptionsFolder}
          isDisabled={false}
        />
      )}
    </StyledContainer>
  ) : (
    canCreate && (
      <ContextMenuButton
        className="add-button"
        directionX="right"
        iconName="images/plus.svg"
        size={17}
        color="#A3A9AE"
        hoverColor="#657077"
        isFill
        getData={getContextOptionsPlus}
        isDisabled={false}
      />
    )
  );
};

ControlButtons.propTypes = {
  personal: PropTypes.bool,
  isRootFolder: PropTypes.bool,
  canCreate: PropTypes.bool,
  getContextOptionsFolder: PropTypes.func,
  getContextOptionsPlus: PropTypes.func,
};

export default React.memo(ControlButtons);
