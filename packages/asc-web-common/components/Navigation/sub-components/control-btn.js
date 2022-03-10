import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import ContextMenuButton from "@appserver/components/context-menu-button";
import IconButton from "@appserver/components/icon-button";
import { isMobile } from "react-device-detect";
import { tablet } from "@appserver/components/utils/device";

const StyledContainer = styled.div`
  margin-left: 20px;
  display: flex;
  align-items: center;

  .add-button {
    margin-right: 12px;
    min-width: 17px;

    ${(props) =>
      !props.isDropBox &&
      css`
        @media ${tablet} {
          display: none;
        }
      `}

    ${isMobile &&
    css`
      ${(props) => !props.isDropBox && "display: none"};
    `}
  }

  .option-button {
    margin-right: 8px;
    min-width: 17px;
  }

  .trash-button {
    min-width: 17px;
  }
`;

const ControlButtons = ({
  personal,
  isDropBox,
  isRootFolder,
  canCreate,
  getContextOptionsFolder,
  getContextOptionsPlus,
  isRecycleBinFolder,
  isEmptyFilesList,
  clearTrash,
}) => {
  return (
    <StyledContainer isDropBox={isDropBox}>
      {!isRootFolder && canCreate ? (
        <>
          <ContextMenuButton
            className="add-button"
            directionX="right"
            iconName="images/plus.svg"
            size={17}
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
              isFill
              getData={getContextOptionsFolder}
              isDisabled={false}
            />
          )}
        </>
      ) : canCreate ? (
        <ContextMenuButton
          className="add-button"
          directionX="right"
          iconName="images/plus.svg"
          size={17}
          isFill
          getData={getContextOptionsPlus}
          isDisabled={false}
        />
      ) : isRecycleBinFolder && !isEmptyFilesList ? (
        <IconButton
          iconName="images/clear.active.react.svg"
          size={17}
          isFill={true}
          onClick={clearTrash}
          className="trash-button"
        />
      ) : (
        <></>
      )}
    </StyledContainer>
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
