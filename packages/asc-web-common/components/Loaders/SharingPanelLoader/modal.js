import React from "react";
import PropTypes from "prop-types";

import {
  StyledContainer,
  StyledHeader,
  StyledExternalLink,
  StyledInternalLink,
  StyledOwner,
  StyledBody,
  StyledItem,
  StyledButtons,
} from "./StyledSharingPanel";
import RectangleLoader from "../RectangleLoader/RectangleLoader";

const SharingPanelLoaderModal = ({
  id,
  className,
  style,
  isShared,
  ...rest
}) => {
  return (
    <StyledContainer>
      <StyledHeader isPersonal={true}>
        <RectangleLoader width={"283px"} height={"16px"} />
      </StyledHeader>

      <StyledExternalLink isPersonal={true}>
        <RectangleLoader
          className="rectangle-loader"
          width={"146px"}
          height={"22px"}
        />
        {isShared && (
          <>
            <RectangleLoader
              className="rectangle-loader"
              width={"368px"}
              height={"32px"}
            />
            <RectangleLoader width={"184px"} height={"20px"} />
          </>
        )}
      </StyledExternalLink>
      <StyledButtons>
        <RectangleLoader width={"100%"} height={"40px"} />
        <RectangleLoader width={"100%"} height={"40px"} />
      </StyledButtons>
    </StyledContainer>
  );
};

SharingPanelLoaderModal.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
};

SharingPanelLoaderModal.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default SharingPanelLoaderModal;
