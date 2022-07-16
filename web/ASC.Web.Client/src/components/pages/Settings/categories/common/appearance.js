import React, { useState, useEffect, useCallback, useMemo } from "react";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";
import Button from "@appserver/components/button";
import withLoading from "../../../../../HOCs/withLoading";
import globalColors from "@appserver/components/utils/globalColors";
import styled, { css } from "styled-components";
import TabContainer from "@appserver/components/tabs-container";
import Preview from "./settingsAppearance/preview";

import ColorSchemeDialog from "./sub-components/colorSchemeDialog";

import DropDownItem from "@appserver/components/drop-down-item";
import DropDownContainer from "@appserver/components/drop-down";

import HexColorPickerComponent from "./sub-components/hexColorPicker";
import { isMobileOnly } from "react-device-detect";
const StyledComponent = styled.div`
  .container {
    display: flex;
  }

  .box {
    width: 46px;
    height: 46px;
    margin-right: 12px;
  }
  #color-scheme_1 {
    background: ${globalColors.colorSchemeDefault_1};
  }
  #color-scheme_2 {
    background: ${globalColors.colorSchemeDefault_2};
  }
  #color-scheme_3 {
    background: ${globalColors.colorSchemeDefault_3};
  }
  #color-scheme_4 {
    background: ${globalColors.colorSchemeDefault_4};
  }
  #color-scheme_5 {
    background: ${globalColors.colorSchemeDefault_5};
  }
  #color-scheme_6 {
    background: ${globalColors.colorSchemeDefault_6};
  }
  #color-scheme_7 {
    background: ${globalColors.colorSchemeDefault_7};
  }

  .add-theme {
    background: #d0d5da;
    padding-top: 16px;
    padding-left: 16px;
    box-sizing: border-box;
  }

  .drop-down-container {
    position: absolute;
    left: -16px;
    bottom: 0;
  }
`;

const StyledComponentDropDownContainer = styled(DropDownContainer)`
  left: -16px;
  bottom: -16px;
  width: 100%;
  border-radius: inherit;
  box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);

  .drop-down-item-hex {
    display: block;
  }

  //TODO:mobile horizontal orientation
  @media (min-width: 428px) {
    ${!isMobileOnly &&
    css`
      left: auto;
      bottom: auto;
      border: none;
      width: auto;
      .drop-down-item-hex {
        display: flex;
      }
    `}
  }
`;

const Appearance = (props) => {
  const { setColorScheme, colorScheme } = props;

  const [selectedColor, setSelectedColor] = useState(1);

  const [previewTheme, setPreviewTheme] = useState("Light theme");

  const [showColorSchemeDialog, setShowColorSchemeDialog] = useState(false);

  const [headerColorSchemeDialog, setHeaderColorSchemeDialog] = useState(
    "Edit color scheme"
  );

  const [currentColorAccent, setCurrentColorAccent] = useState(null);
  const [currentColorButtons, setCurrentColorButtons] = useState(null);

  const [openHexColorPickerAccent, setOpenHexColorPickerAccent] = useState(
    false
  );
  const [openHexColorPickerButtons, setOpenHexColorPickerButtons] = useState(
    false
  );

  //TODO: Add default color
  const [appliedColorAccent, setAppliedColorAccent] = useState("#FF9933");
  const [appliedColorButtons, setAppliedColorButtons] = useState("#FF9999");

  const checkImg = <img src="/static/images/check.white.svg" />;

  const array_items = useMemo(
    () => [
      {
        key: "0",
        title: "Light theme",
        content: (
          <Preview previewTheme={previewTheme} selectedColor={selectedColor} />
        ),
      },
      {
        key: "1",
        title: "Dark theme",
        content: (
          <Preview previewTheme={previewTheme} selectedColor={selectedColor} />
        ),
      },
    ],
    [selectedColor, previewTheme]
  );

  const onColorSelection = (e) => {
    if (!e.target.id) return;

    const colorNumber = e.target.id[e.target.id.length - 1];
    setSelectedColor(+colorNumber);
  };

  const onShowCheck = (colorNumber) => {
    return selectedColor && selectedColor === colorNumber && checkImg;
  };

  const onChangePreviewTheme = (e) => {
    setPreviewTheme(e.title);
  };

  const onSaveSelectedColor = () => {
    setColorScheme(selectedColor);
  };

  const onClickEdit = () => {
    // TODO: give store Accent color and Buttons main to id
    setCurrentColorAccent("#F97A0B");
    setCurrentColorButtons("#FF9933");

    setHeaderColorSchemeDialog("Edit color scheme");

    setShowColorSchemeDialog(true);
  };

  const onCloseEdit = () => {
    setShowColorSchemeDialog(false);
  };

  const onAddTheme = () => {
    setCurrentColorAccent(
      "url(/static/images/plus.theme.svg) 15px 15px no-repeat #D0D5DA"
    );
    setCurrentColorButtons(
      "url(/static/images/plus.theme.svg) 15px 15px no-repeat #D0D5DA"
    );

    setHeaderColorSchemeDialog("New color scheme");

    setShowColorSchemeDialog(true);
  };

  const onClickColor = (e) => {
    if (e.target.id === "accent") {
      setOpenHexColorPickerAccent(true);
    } else {
      setOpenHexColorPickerButtons(true);
    }
  };

  const onCloseHexColorPicker = () => {
    setOpenHexColorPickerAccent(false);
    setOpenHexColorPickerButtons(false);
  };

  const onAppliedColorAccent = () => {
    setCurrentColorAccent(appliedColorAccent);
    onCloseHexColorPicker();
  };

  const onAppliedColorButtons = () => {
    setCurrentColorButtons(appliedColorButtons);
    onCloseHexColorPicker();
  };

  const nodeHexColorPickerButtons = (
    <StyledComponentDropDownContainer
      directionX="right"
      withBackdrop={false}
      isDefaultMode={false}
      open={openHexColorPickerButtons}
      clickOutsideAction={onCloseHexColorPicker}
    >
      <DropDownItem className="drop-down-item-hex">
        <HexColorPickerComponent
          id="buttons-hex"
          onCloseHexColorPicker={onCloseHexColorPicker}
          onAppliedColor={onAppliedColorButtons}
          color={appliedColorButtons}
          setColor={setAppliedColorButtons}
        />
      </DropDownItem>
    </StyledComponentDropDownContainer>
  );

  const nodeHexColorPickerAccent = (
    <StyledComponentDropDownContainer
      directionX="right"
      withBackdrop={false}
      isDefaultMode={false}
      open={openHexColorPickerAccent}
      clickOutsideAction={onCloseHexColorPicker}
    >
      <DropDownItem className="drop-down-item-hex">
        <HexColorPickerComponent
          id="accent-hex"
          onCloseHexColorPicker={onCloseHexColorPicker}
          onAppliedColor={onAppliedColorAccent}
          color={appliedColorAccent}
          setColor={setAppliedColorAccent}
        />
      </DropDownItem>
    </StyledComponentDropDownContainer>
  );

  const nodeAccentColor = (
    <div
      id="accent"
      style={{ background: currentColorAccent }}
      className="color-button"
      onClick={onClickColor}
    ></div>
  );

  const nodeButtonsColor = (
    <div
      id="buttons"
      style={{ background: currentColorButtons }}
      className="color-button"
      onClick={onClickColor}
    ></div>
  );

  return (
    <StyledComponent>
      <div>Color</div>

      <div className="container">
        <div id="color-scheme_1" className="box" onClick={onColorSelection}>
          {onShowCheck(1)}
        </div>
        <div id="color-scheme_2" className="box" onClick={onColorSelection}>
          {onShowCheck(2)}
        </div>
        <div id="color-scheme_3" className="box" onClick={onColorSelection}>
          {onShowCheck(3)}
        </div>
        <div id="color-scheme_4" className="box" onClick={onColorSelection}>
          {onShowCheck(4)}
        </div>
        <div id="color-scheme_5" className="box" onClick={onColorSelection}>
          {onShowCheck(5)}
        </div>
        <div id="color-scheme_6" className="box" onClick={onColorSelection}>
          {onShowCheck(6)}
        </div>
        <div id="color-scheme_7" className="box" onClick={onColorSelection}>
          {onShowCheck(7)}
        </div>
        <div className="add-theme box" onClick={onAddTheme}>
          <img src="/static/images/plus.theme.svg" />
        </div>
      </div>

      <div onClick={onClickEdit}>Edit color scheme</div>
      <ColorSchemeDialog
        nodeButtonsColor={nodeButtonsColor}
        nodeAccentColor={nodeAccentColor}
        nodeHexColorPickerAccent={nodeHexColorPickerAccent}
        nodeHexColorPickerButtons={nodeHexColorPickerButtons}
        visible={showColorSchemeDialog}
        onClose={onCloseEdit}
        header={headerColorSchemeDialog}
      />
      <div>Preview</div>
      <TabContainer elements={array_items} onSelect={onChangePreviewTheme} />
      <Button label="Save" onClick={onSaveSelectedColor} primary size="small" />
    </StyledComponent>
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { colorScheme, setColorScheme } = settingsStore;

  return {
    colorScheme,
    setColorScheme,
  };
})(observer(Appearance));
