import React from "react";
import {
  CloudLeft,
  CloudRight,
  ConfettiLeft,
  ConfettiRight,
  IllustrationSvg,
} from "./svg";

import styled from "styled-components";

const StyledIllustration = styled.div`
  display: flex;
  position: relative;

  h1 {
    font-size: 44px;
    z-index: 42;
    text-align: center;
  }

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
    left: -140px;
    z-index: 0;
  }

  .cloud-right {
    top: 0;
    right: -80px;
    z-index: 0;
  }

  @media (max-width: 1024px) {
    margin: 0 auto 40px;

    .cloud-left,
    .cloud-right {
      display: none;
    }

    .illustration-svg {
      width: 100%;
    }
  }

  @media (max-width: 768px) {
    display: none;
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
