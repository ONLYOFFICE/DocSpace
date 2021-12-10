import React from "react";
import {
  CloudLeft,
  CloudRight,
  ConfettiLeft,
  ConfettiRight,
  IllustrationSvg,
} from "./svg";

import styled, { css } from "styled-components";
import { isMobile } from "react-device-detect";

const StyledIllustration = styled.div`
  display: ${isMobile ? "none" : "flex"};
  position: relative;
  max-height: 308px;

  .illustration-svg {
    width: 500px;
    z-index: 42;
  }

  .confetti-right,
  .confetti-left,
  .cloud-left,
  .cloud-right {
    position: absolute;
  }

  .confetti-right {
    top: 20px;
    right: 0;
    z-index: 1;
  }

  .confetti-left {
    top: 20px;
    left: 10px;
    z-index: 1;
  }

  .cloud-left {
    bottom: 0;
    left: -75px;
    z-index: 0;
  }

  .cloud-right {
    top: 0;
    right: -120px;
    z-index: 0;
  }

  ${isMobile &&
  css`
    background-color: antiquewhite;
  `}

  @media (max-width: 1024px) {
    margin: 0 auto 40px;
    order: 1;

    .cloud-left,
    .cloud-right {
      display: none;
    }

    .illustration-svg {
      width: 100%;
    }
  }
`;

const HomeIllustration = () => {
  return (
    <StyledIllustration>
      <ConfettiLeft className="confetti-left" />
      <ConfettiRight className="confetti-right" />
      <CloudLeft className="cloud-left" />
      <CloudRight className="cloud-right" />
      <IllustrationSvg className="illustration-svg" />
    </StyledIllustration>
  );
};

export default HomeIllustration;
