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
} from "./StyledSharingPanel";
import RectangleLoader from "../RectangleLoader/RectangleLoader";

const SharingPanelLoader = ({ id, className, style, ...rest }) => {
  return (
    <StyledContainer>
      <StyledHeader>
        <RectangleLoader width={"283px"} height={"29px"} />
        <RectangleLoader width={"48px"} height={"29px"} />
      </StyledHeader>

      <StyledExternalLink>
        <RectangleLoader
          className="rectangle-loader"
          width={"146px"}
          height={"22px"}
        />
        <RectangleLoader
          className="rectangle-loader"
          width={"448px"}
          height={"32px"}
        />
        <RectangleLoader width={"184px"} height={"20px"} />
      </StyledExternalLink>

      <StyledInternalLink>
        <RectangleLoader width={"99px"} height={"22px"} />
        <RectangleLoader width={"30px"} height={"22px"} />
      </StyledInternalLink>

      <StyledOwner>
        <div className="owner-info">
          <RectangleLoader
            width={"32px"}
            height={"32px"}
            borderRadius={"1000px"}
          />
          <RectangleLoader width={"91px"} height={"16px"} />
        </div>
        <RectangleLoader width={"91px"} height={"16px"} />
      </StyledOwner>

      <StyledBody>
        <StyledItem>
          <div className="item-info">
            <RectangleLoader
              width={"32px"}
              height={"32px"}
              borderRadius={"1000px"}
            />
            <RectangleLoader width={"91px"} height={"16px"} />
          </div>
          <RectangleLoader width={"45px"} height={"32px"} />
        </StyledItem>
        <StyledItem>
          <div className="item-info">
            <RectangleLoader
              width={"32px"}
              height={"32px"}
              borderRadius={"1000px"}
            />
            <RectangleLoader width={"91px"} height={"16px"} />
          </div>
          <RectangleLoader width={"45px"} height={"32px"} />
        </StyledItem>
        <StyledItem>
          <div className="item-info">
            <RectangleLoader
              width={"32px"}
              height={"32px"}
              borderRadius={"1000px"}
            />
            <RectangleLoader width={"91px"} height={"16px"} />
          </div>
          <RectangleLoader width={"45px"} height={"32px"} />
        </StyledItem>
        <StyledItem>
          <div className="item-info">
            <RectangleLoader
              width={"32px"}
              height={"32px"}
              borderRadius={"1000px"}
            />
            <RectangleLoader width={"91px"} height={"16px"} />
          </div>
          <RectangleLoader width={"45px"} height={"32px"} />
        </StyledItem>
        <StyledItem>
          <div className="item-info">
            <RectangleLoader
              width={"32px"}
              height={"32px"}
              borderRadius={"1000px"}
            />
            <RectangleLoader width={"91px"} height={"16px"} />
          </div>
          <RectangleLoader width={"45px"} height={"32px"} />
        </StyledItem>
        <StyledItem>
          <div className="item-info">
            <RectangleLoader
              width={"32px"}
              height={"32px"}
              borderRadius={"1000px"}
            />
            <RectangleLoader width={"91px"} height={"16px"} />
          </div>
          <RectangleLoader width={"45px"} height={"32px"} />
        </StyledItem>
      </StyledBody>
    </StyledContainer>
  );
};

SharingPanelLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
};

SharingPanelLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default SharingPanelLoader;
