import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import Badge from "@docspace/components/badge";
import { inject, observer } from "mobx-react";
import Loaders from "@docspace/common/components/Loaders";
import MainButton from "@docspace/components/main-button";
import ContextMenuButton from "@docspace/components/context-menu-button";

import { Base } from "@docspace/components/themes";
const StyledComponent = styled.div`
  display: flex;

  .menu {
    display: flex;
    flex-direction: column;
    padding: 21px 0px 17px;
    height: 100%;
    background: ${(props) =>
      props.previewTheme === "Light theme" ? "#f8f9f9" : "#292929"};
    border-width: 1px;
    border-style: solid;
    border-radius: 16px 0px 0px 16px;
  }

  .section {
    width: 323px;
    border-width: 1px;
    border-style: solid;
    border-left-style: none;
    border-radius: 0px 16px 16px 0px;
    background: ${(props) =>
      props.previewTheme === "Light theme" ? "#FFFFFF" : "#333333"};
  }

  .section-header {
    display: flex;
    align-items: flex-start;
    padding: 26px 0px 28px 20px;
  }

  .section-header-loader {
    padding-right: 17px;
    height: 16px;
  }

  .section-search {
    height: 30px;
    border-width: 1px;
    border-style: solid;
    border-radius: 3px 0px 0px 3px;
    border-right-style: none;
    margin: 0px 0px 24px 20px;
  }

  .section-search-loader {
    padding-top: 9px;
    padding-left: 8px;
  }

  .loader-search {
    margin-bottom: 2px;
  }

  .main-button-container {
    margin: 0px 20px 24px;
  }

  .main-button-preview {
    cursor: auto;
  }

  .color,
  .color-badge > div {
    background: ${(props) => props.color};
  }

  .color-loaders rect {
    fill: ${(props) =>
      props.previewTheme === "Light theme"
        ? `${props.color} !important`
        : `#FFFFFF !important`};
  }

  .menu-section {
    &:not(:last-child) {
      padding-bottom: 26px;
    }
  }

  .header {
    margin: 0px 20px 23px 20px;
    height: 24px;
  }

  .loaders-theme {
    background-color: ${(props) =>
      props.previewTheme === "Light theme" ? "#FFF" : "#858585"};
    border-radius: 3px;
  }

  .flex {
    display: flex;
    align-items: center;

    padding: 10px 20px 0px;

    &:not(:last-child) {
      padding-bottom: 10px;
    }
  }

  .padding-right {
    height: 16px;
    padding-right: 8px;
  }

  .title-section {
    height: 12px;
    margin: 0px 20px 4px;
  }

  .menu-badge {
    padding-left: 93px;
    border: none;
    cursor: auto;
  }

  .select {
    background: ${(props) =>
      props.previewTheme === "Light theme" ? "#f0f0f0" : "#333333"};
  }

  .section-tile {
    padding: 0px 20px 0px;
  }

  .border-color {
    border-color: ${(props) =>
      props.previewTheme === "Light theme" ? "#d0d5da" : "#474747"};
  }

  .tile {
    border-width: 1px;
    border-style: solid;
    border-radius: 12px;
    margin-bottom: 15px;
  }

  .background {
    background: ${(props) =>
      props.previewTheme === "Light theme" ? "#FFF" : "#292929"};
  }

  .tile-name {
    display: flex;
    align-items: center;
    width: max-content;
    padding: 16px 22px 16px 16px;
    height: 30px;
  }

  .half {
    border-top-width: 1px;
    border-right-width: 1px;
    border-left-width: 1px;
    border-style: solid;
    border-bottom: none;
    border-radius: 12px 12px 0px 0px;
  }

  .action-button {
    width: 66px;
    display: flex;
    justify-content: flex-end;
    align-items: center;
  }

  .tile-tag {
    display: flex;
    border-top-width: 1px;
    border-top-style: solid;
    padding: 16px 0px 16px 16px;
  }

  .tile-title {
    padding-right: 84px;
  }

  .tile-icon {
    padding-right: 12px;
  }

  .section-badge {
    border: none;
    cursor: auto;
    padding-right: 10px;
  }

  .pin {
    padding-right: 4px;

    path {
      fill: ${(props) =>
        props.previewTheme === "Light theme"
          ? `${props.color} !important`
          : `#FFFFFF !important`};
    }
  }

  .menu-button > div {
    cursor: auto;
  }
`;

StyledComponent.defaultProps = { theme: Base };
const Preview = (props) => {
  const { selectAccentColor, previewTheme } = props;

  const [color, setColor] = useState();

  useEffect(() => {
    setColor(selectAccentColor);
  }, [selectAccentColor]);

  return (
    <StyledComponent color={color} previewTheme={previewTheme}>
      <div className="menu border-color">
        <div className="header">
          <Loaders.Rectangle
            width="211"
            height="24"
            className="loaders-theme"
          />
        </div>

        <div className="main-button-container">
          <MainButton text="Actions" className="main-button-preview color" />
        </div>

        <div className="menu-section">
          <div className="title-section">
            <Loaders.Rectangle
              width="37"
              height="12"
              className="loaders-theme"
            />
          </div>

          <div className="flex">
            <div className="padding-right">
              <Loaders.Rectangle
                width="16"
                height="16"
                className="loaders-theme"
              />
            </div>

            <Loaders.Rectangle
              width="48"
              height="8"
              className="loaders-theme"
            />
          </div>
          <div className="flex select">
            <div className="padding-right">
              <Loaders.Rectangle
                width="16"
                height="16"
                className="color-loaders"
              />
            </div>

            <Loaders.Rectangle
              width="48"
              height="8"
              className="color-loaders"
            />
            <Badge
              className="menu-badge color-badge"
              label={21}
              fontSize="11px"
              fontWeight={800}
              borderRadius="11px"
              padding="0 5px"
              lineHeight="1.46"
            />
          </div>
          <div className="flex">
            <div className="padding-right">
              <Loaders.Rectangle
                width="16"
                height="16"
                className="loaders-theme"
              />
            </div>
            <Loaders.Rectangle
              width="48"
              height="8"
              className="loaders-theme"
            />
          </div>
        </div>

        <div className="menu-section">
          <div className="title-section">
            <Loaders.Rectangle
              width="37"
              height="12"
              className="loaders-theme"
            />
          </div>

          <div className="flex">
            <div className="padding-right">
              <Loaders.Rectangle
                width="16"
                height="16"
                className="loaders-theme"
              />
            </div>

            <Loaders.Rectangle
              width="48"
              height="8"
              className="loaders-theme"
            />
          </div>
        </div>
      </div>

      <div className="section border-color">
        <div className="section-header">
          <div className="section-header-loader">
            <Loaders.Rectangle
              width="60"
              height="16"
              className="loaders-theme"
            />
          </div>

          <img src="/static/images/plus.preview.svg" />
        </div>
        <div className="section-search background border-color">
          <div className="section-search-loader">
            <Loaders.Rectangle
              width="48"
              height="12"
              className="loaders-theme loader-search"
            />
          </div>
        </div>
        <div className="section-tile">
          <div className="tile background border-color">
            <div className="tile-name">
              <div className="tile-icon">
                <Loaders.Rectangle
                  width="32"
                  height="32"
                  className="loaders-theme"
                />
              </div>

              <div className="tile-title">
                <Loaders.Rectangle
                  width="48"
                  height="10"
                  className="loaders-theme"
                />
              </div>

              <div className="action-button">
                <Badge
                  className="section-badge color-badge"
                  label={3}
                  fontSize="11px"
                  fontWeight={800}
                  borderRadius="11px"
                  padding="0 5px"
                  lineHeight="1.46"
                />
                {/* <img src="/static/images/active.pin.svg" className="pin" />
                <object
                  type="image/svg+xml"
                  className="pin"
                  data="/static/images/active.pin.svg"
                ></object> */}
                {/* TODO: Сhange picture */}
                <svg
                  className="pin"
                  width="12"
                  height="16"
                  viewBox="0 0 12 16"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M9.5783 -0.000242493L2.41936 -0.000241966C2.1608 -0.000374392 1.95098 0.209442 1.95111 0.468004L1.95111 0.498338C1.95105 0.937244 2.12199 1.35006 2.43234 1.66041C2.63229 1.86036 2.87496 2.00143 3.13948 2.07719L2.60851 7.27556C2.06569 7.30602 1.55963 7.53167 1.17212 7.91918C0.754536 8.33676 0.524586 8.8919 0.524652 9.48234L0.524652 9.52725C0.524586 9.78587 0.73427 9.99556 0.992898 9.99549L4.99937 9.99549L4.9993 13.5101C5.0005 13.5949 5.17017 15.0259 5.19137 15.2188C5.21258 15.4118 5.36324 15.5487 5.36417 15.5624C5.38013 15.8082 5.75521 15.9992 6.00178 15.9998C6.13093 16 6.41664 15.9478 6.50187 15.8625C6.57956 15.7848 6.63029 15.6798 6.63818 15.5624C6.6389 15.5521 6.79437 15.3697 6.81097 15.2188C6.82757 15.068 7.00165 13.595 7.00284 13.5036V9.99562L11.0049 9.99562C11.1342 9.99569 11.2513 9.94324 11.336 9.85853C11.4207 9.77382 11.4733 9.65666 11.4731 9.52738V9.48247C11.4732 8.89203 11.2432 8.33683 10.8257 7.91931C10.4382 7.5318 9.93203 7.30622 9.38928 7.27569L8.85831 2.07733C9.12283 2.00156 9.3655 1.86049 9.56545 1.66054C9.87587 1.35012 10.0468 0.937442 10.0467 0.49847L10.0467 0.468136C10.0466 0.209376 9.83692 -0.000308579 9.5783 -0.000242493Z"
                  />
                </svg>
                <ContextMenuButton directionX="right" className="menu-button" />
              </div>
            </div>
            <div className="tile-tag border-color">
              <Loaders.Rectangle
                width="63"
                height="24"
                className="loaders-theme"
              />
            </div>
          </div>
          <div className="tile-name half background border-color">
            <div className="tile-icon">
              <Loaders.Rectangle
                width="32"
                height="32"
                className="loaders-theme"
              />
            </div>

            <div className="tile-title">
              <Loaders.Rectangle
                width="48"
                height="10"
                className="loaders-theme"
              />
            </div>

            <div className="action-button">
              {/* <img src="/static/images/active.pin.svg" className="pin" /> */}
              {/* TODO: Сhange picture */}
              <svg
                className="pin"
                width="12"
                height="16"
                viewBox="0 0 12 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M9.5783 -0.000242493L2.41936 -0.000241966C2.1608 -0.000374392 1.95098 0.209442 1.95111 0.468004L1.95111 0.498338C1.95105 0.937244 2.12199 1.35006 2.43234 1.66041C2.63229 1.86036 2.87496 2.00143 3.13948 2.07719L2.60851 7.27556C2.06569 7.30602 1.55963 7.53167 1.17212 7.91918C0.754536 8.33676 0.524586 8.8919 0.524652 9.48234L0.524652 9.52725C0.524586 9.78587 0.73427 9.99556 0.992898 9.99549L4.99937 9.99549L4.9993 13.5101C5.0005 13.5949 5.17017 15.0259 5.19137 15.2188C5.21258 15.4118 5.36324 15.5487 5.36417 15.5624C5.38013 15.8082 5.75521 15.9992 6.00178 15.9998C6.13093 16 6.41664 15.9478 6.50187 15.8625C6.57956 15.7848 6.63029 15.6798 6.63818 15.5624C6.6389 15.5521 6.79437 15.3697 6.81097 15.2188C6.82757 15.068 7.00165 13.595 7.00284 13.5036V9.99562L11.0049 9.99562C11.1342 9.99569 11.2513 9.94324 11.336 9.85853C11.4207 9.77382 11.4733 9.65666 11.4731 9.52738V9.48247C11.4732 8.89203 11.2432 8.33683 10.8257 7.91931C10.4382 7.5318 9.93203 7.30622 9.38928 7.27569L8.85831 2.07733C9.12283 2.00156 9.3655 1.86049 9.56545 1.66054C9.87587 1.35012 10.0468 0.937442 10.0467 0.49847L10.0467 0.468136C10.0466 0.209376 9.83692 -0.000308579 9.5783 -0.000242493Z"
                />
              </svg>
              <ContextMenuButton directionX="right" className="menu-button" />
            </div>
          </div>
        </div>
      </div>
    </StyledComponent>
  );
};

export default inject(({ auth, setup, common }) => {})(
  withTranslation(["Settings", "Common"])(observer(Preview))
);
