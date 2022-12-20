import React from "react";
import styled, { css } from "styled-components";
import Base from "../themes/base";

import CrossIcon from "@docspace/components/public/static/images/cross.react.svg";

import { isMobile } from "react-device-detect";

import { tablet } from "@docspace/components/utils/device";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
  visible,
  scale,
  zIndex,
  contentPaddingBottom,
  ...props
}) => <aside {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledAside = styled(Container)`
  background-color: ${(props) => props.theme.aside.backgroundColor};
  height: ${(props) => props.theme.aside.height};

  position: fixed;
  right: ${(props) => props.theme.aside.right};
  top: ${(props) => props.theme.aside.top};
  transform: translateX(
    ${(props) => (props.visible ? "0" : props.scale ? "100%" : "480px")}
  );
  transition: ${(props) => props.theme.aside.transition};
  width: ${(props) => (props.scale ? "100%" : "480px")};
  z-index: ${(props) => props.zIndex};
  box-sizing: border-box;

  @media ${tablet} {
    max-width: calc(100% - 69px);
    transform: translateX(
      ${(props) => (props.visible ? "0" : props.scale ? "100%" : "480px")}
    );
  }

  ${isMobile &&
  css`
    max-width: calc(100% - 69px);
    transform: translateX(
      ${(props) => (props.visible ? "0" : props.scale ? "100%" : "480px")}
    );
  `}

  @media (max-width: 428px) {
    bottom: 0;
    top: unset;
    height: calc(100% - 64px);
    width: 100%;
    max-width: 100%;
    transform: translateX(${(props) => (props.visible ? "0" : "100%")});

    aside:not(:first-child) {
      height: 100%;
    }
  }

  &.modal-dialog-aside {
    padding-bottom: ${(props) =>
      props.contentPaddingBottom
        ? props.contentPaddingBottom
        : props.theme.aside.paddingBottom};

    .modal-dialog-aside-footer {
      position: fixed;
      bottom: ${(props) => props.theme.aside.bottom};
    }
  }
`;
StyledAside.defaultProps = { theme: Base };

const StyledControlContainer = styled.div`
  display: none;

  width: 17px;
  height: 17px;
  position: absolute;

  cursor: pointer;

  align-items: center;
  justify-content: center;
  z-index: 450;

  @media ${tablet} {
    display: flex;

    top: 18px;
    left: -27px;
  }

  ${isMobile &&
  css`
    display: flex;

    top: 18px;
    left: -27px;
  `}

  @media (max-width: 428px) {
    display: flex;

    top: -27px;
    right: 10px;
    left: unset;
  }
`;

StyledControlContainer.defaultProps = { theme: Base };

const StyledCrossIcon = styled(CrossIcon)`
  width: 17px;
  height: 17px;
  z-index: 455;
  path {
    fill: ${(props) => props.theme.catalog.control.fill};
  }
`;

StyledCrossIcon.defaultProps = { theme: Base };

export { StyledAside, StyledControlContainer, StyledCrossIcon };
