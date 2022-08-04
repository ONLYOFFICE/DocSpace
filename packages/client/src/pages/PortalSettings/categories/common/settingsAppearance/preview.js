import React, { useState, useEffect } from "react";

import styled from "styled-components";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { withTranslation } from "react-i18next";
import Badge from "@docspace/components/badge";
import { inject, observer } from "mobx-react";
import Loaders from "@docspace/common/components/Loaders";
import MainButton from "@docspace/components/main-button";
import ContextMenuButton from "@docspace/components/context-menu-button";
import globalColors from "@docspace/components/utils/globalColors";

const StyledComponent = styled.div`
  display: flex;

  .menu {
    display: flex;
    flex-direction: column;
    padding: 21px 0px 17px;

    background: #f8f9f9;
    border: 1px solid #d0d5da;
    border-radius: 16px 0px 0px 16px;
  }

  .section {
    width: 323px;
    border: 1px solid #d0d5da;
    border-left-style: none;
    border-radius: 0px 16px 16px 0px;
  }

  .section-header {
    display: flex;
    padding: 26px 0px 28px 20px;
  }

  .section-header-loader {
    padding-right: 17px;
  }

  .section-search {
    height: 32px;
    border: 1px solid #d0d5da;
    border-radius: 3px 0px 0px 3px;
    border-right-style: none;
    margin: 0px 0px 25px 20px;
  }

  .section-search-loader {
    padding: 10px 0px 10px 8px;
  }

  .main-button-preview {
    cursor: auto;
    margin: 0px 10px 12px;
  }

  .menu-section {
    &:not(:last-child) {
      padding-bottom: 26px;
    }
  }

  .header {
    padding: 0px 20px 23px 20px;
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
    padding-right: 8px;
  }

  .title-section {
    padding: 0px 20px 4px;
  }

  .menu-badge {
    padding-left: 93px;
    border: none;
    cursor: auto;
  }

  .select {
    background: #f0f0f0;
  }

  .section-tile {
    padding: 0px 20px 0px;
  }

  .tile {
    border: 1px solid #eceef1;
    border-radius: 12px;
    margin-bottom: 16px;
  }

  .tile-name {
    display: flex;
    align-items: center;
    width: max-content;
    padding: 16px 22px 16px 16px;
  }

  .half {
    border-top: 1px solid #eceef1;
    border-right: 1px solid #eceef1;
    border-left: 1px solid #eceef1;
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
    border-top: 1px solid #eceef1;
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
  }

  .menu-button > div {
    cursor: auto;
  }
`;

const Preview = (props) => {
  const { selectedColor } = props;

  const [color, setColor] = useState();
  const globalColors = "globalColors.colorSchemeDefault_";

  useEffect(() => {
    setColor(globalColors + `${selectedColor}`);
  }, [selectedColor]);

  return (
    <StyledComponent>
      <div className="menu">
        <Loaders.Rectangle
          width="211"
          height="24"
          className="header"
          style={{ background: `${color}` }}
        />

        <MainButton
          text="Actions"
          className="main-button-preview"
          style={{ background: `${color}` }}
        />

        <div className="menu-section">
          <Loaders.Rectangle width="37" height="12" className="title-section" />

          <div className="flex">
            <Loaders.Rectangle
              width="16"
              height="16"
              className="padding-right"
            />
            <Loaders.Rectangle width="48" height="8" />
          </div>
          <div className="flex select">
            <Loaders.Rectangle
              width="16"
              height="16"
              className="padding-right"
            />
            <Loaders.Rectangle width="48" height="8" />
            <Badge
              className="menu-badge"
              label={21}
              fontSize="11px"
              fontWeight={800}
              borderRadius="11px"
              padding="0 5px"
              lineHeight="1.46"
            />
          </div>
          <div className="flex">
            <Loaders.Rectangle
              width="16"
              height="16"
              className="padding-right"
            />
            <Loaders.Rectangle width="48" height="8" />
          </div>
        </div>

        <div className="menu-section">
          <Loaders.Rectangle width="37" height="12" className="title-section" />
          <div className="flex">
            <Loaders.Rectangle
              width="16"
              height="16"
              className="padding-right"
            />
            <Loaders.Rectangle width="48" height="8" />
          </div>
        </div>
      </div>

      <div className="section">
        <div className="section-header">
          <Loaders.Rectangle
            width="60"
            height="16"
            className="section-header-loader"
          />
          <img src="/static/images/plus.preview.svg" />
        </div>
        <div className="section-search">
          <Loaders.Rectangle
            width="48"
            height="12"
            className="section-search-loader"
          />
        </div>
        <div className="section-tile">
          <div className="tile">
            <div className="tile-name">
              <Loaders.Rectangle width="32" height="32" className="tile-icon" />
              <Loaders.Rectangle
                width="48"
                height="10"
                className="tile-title"
              />

              <div className="action-button">
                <Badge
                  className="section-badge"
                  label={3}
                  fontSize="11px"
                  fontWeight={800}
                  borderRadius="11px"
                  padding="0 5px"
                  lineHeight="1.46"
                />
                <img src="/static/images/active.pin.svg" className="pin" />
                <ContextMenuButton directionX="right" className="menu-button" />
              </div>
            </div>
            <div className="tile-tag">
              <Loaders.Rectangle width="63" height="24" />
            </div>
          </div>
          <div className="tile-name  half">
            <Loaders.Rectangle width="32" height="32" className="tile-icon" />
            <Loaders.Rectangle width="48" height="10" className="tile-title" />

            <div className="action-button">
              <img src="/static/images/active.pin.svg" className="pin" />
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
