import React, { useState, useEffect, useCallback, useMemo } from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import toastr from "@docspace/components/toast/toastr";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import withLoading from "SRC_DIR/HOCs/withLoading";

import styled, { css } from "styled-components";
import TabContainer from "@docspace/components/tabs-container";
import Preview from "./settingsAppearance/preview";

import ColorSchemeDialog from "./sub-components/colorSchemeDialog";

import DropDownItem from "@docspace/components/drop-down-item";
import DropDownContainer from "@docspace/components/drop-down";

import HexColorPickerComponent from "./sub-components/hexColorPicker";
import { isMobileOnly } from "react-device-detect";

import Loaders from "@docspace/common/components/Loaders";

import { StyledComponent } from "./settingsAppearance/StyledApperance.js";

import BreakpointWarning from "../../../../BreakpointWarning/index";

const Appearance = (props) => {
  const {
    appearanceTheme,
    selectedThemeId,
    sendAppearanceTheme,
    getAppearanceTheme,
    currentColorScheme,
    t,
  } = props;

  // const [selectedColor, setSelectedColor] = useState(selectedThemeId);

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
  const [appliedColorAccent, setAppliedColorAccent] = useState("#F97A0B");
  const [appliedColorButtons, setAppliedColorButtons] = useState("#FF9933");

  const [changeCurrentColorAccent, setChangeCurrentColorAccent] = useState(
    false
  );
  const [changeCurrentColorButtons, setChangeCurrentColorButtons] = useState(
    false
  );

  const [viewMobile, setViewMobile] = useState(false);

  const [showSaveButtonDialog, setShowSaveButtonDialog] = useState(false);
  const [
    showRestoreToDefaultButtonDialog,
    setShowRestoreToDefaultButtonDialog,
  ] = useState(false);

  const [isEditDialog, setIsEditDialog] = useState(false);
  const [isAddThemeDialog, setIsAddThemeDialog] = useState(false);

  const [selectAccentColor, setSelectAccentColor] = useState(
    currentColorScheme.accentColor
  );
  const [selectThemeId, setSelectThemeId] = useState(selectedThemeId);

  const [changeTheme, setChangeTheme] = useState([]);

  const checkImg = (
    <img className="check-img" src="/static/images/check.white.svg" />
  );

  const array_items = useMemo(
    () => [
      {
        key: "0",
        title: t("Profile:LightTheme"),
        content: (
          <Preview
            previewTheme={previewTheme}
            selectAccentColor={selectAccentColor}
            selectThemeId={selectThemeId}
            themePreview="Light"
          />
        ),
      },
      {
        key: "1",
        title: t("Profile:DarkTheme"),
        content: (
          <Preview
            previewTheme={previewTheme}
            selectAccentColor={selectAccentColor}
            selectThemeId={selectThemeId}
            themePreview="Dark"
          />
        ),
      },
    ],
    [selectAccentColor, previewTheme, selectThemeId]
  );

  useEffect(() => {
    onCheckView();
    window.addEventListener("resize", onCheckView);

    return () => window.removeEventListener("resize", onCheckView);
  }, []);

  useEffect(() => {
    if (
      changeCurrentColorAccent &&
      changeCurrentColorButtons &&
      isAddThemeDialog
    ) {
      setShowSaveButtonDialog(true);
    }

    if (
      (changeCurrentColorAccent || changeCurrentColorButtons) &&
      isEditDialog
    ) {
      setShowSaveButtonDialog(true);
    }
  }, [
    changeCurrentColorAccent,
    changeCurrentColorButtons,
    isAddThemeDialog,
    isEditDialog,
  ]);

  const onCheckView = () => {
    if (isMobileOnly || window.innerWidth < 600) {
      setViewMobile(true);
    } else {
      setViewMobile(false);
    }
  };

  const onColorSelection = (item) => {
    setSelectAccentColor(item.accentColor);
    setSelectThemeId(item.id);
    //TODO: find id and give item
    //TODO: if changeTheme array = 0, then appearanceTheme, else changeTheme array
    // const theme = changeTheme?.find((item) => item.id === colorNumber);
    // If theme has already been edited before
    // if (theme) {
    //   theme.isSelected = true;
    //   setSelectThemeId(theme);
    // } else {
    //    If theme not has already been edited before
    //   const theme = appearanceTheme.find((item) => item.id === colorNumber);
    //   theme.isSelected = true;
    //   setSelectThemeId(theme);
    // }
  };

  const onShowCheck = (colorNumber) => {
    return selectThemeId && selectThemeId === colorNumber && checkImg;
  };

  const onChangePreviewTheme = (e) => {
    setPreviewTheme(e.title);
  };

  const onSaveSelectedColor = () => {
    sendAppearanceTheme({ selected: selectThemeId })
      .then(() => {
        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
        getAppearanceTheme();
      })
      .catch((error) => {
        toastr.error(error);
      });
  };

  const onClickEdit = () => {
    appearanceTheme.map((item) => {
      if (item.id === selectThemeId) {
        // TODO: give store Accent color and Buttons main to id

        setCurrentColorAccent(item.accentColor);
        setCurrentColorButtons(item.buttonsMain);
      }
    });

    setIsEditDialog(true);

    setHeaderColorSchemeDialog("Edit color scheme");

    //TODO: if position <=7 then default theme and show button RestoreToDefault
    setShowRestoreToDefaultButtonDialog(true);

    setShowColorSchemeDialog(true);
  };

  const onCloseColorSchemeDialog = () => {
    setShowColorSchemeDialog(false);

    setOpenHexColorPickerAccent(false);
    setOpenHexColorPickerButtons(false);

    setChangeCurrentColorAccent(false);
    setChangeCurrentColorButtons(false);

    setIsEditDialog(false);
    setIsAddThemeDialog(false);

    setShowSaveButtonDialog(false);
  };

  const onAddTheme = () => {
    setIsAddThemeDialog(true);
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
      setOpenHexColorPickerButtons(false);
    } else {
      setOpenHexColorPickerButtons(true);
      setOpenHexColorPickerAccent(false);
    }
  };

  const onCloseHexColorPicker = () => {
    setOpenHexColorPickerAccent(false);
    setOpenHexColorPickerButtons(false);
  };

  const onAppliedColorAccent = useCallback(() => {
    setCurrentColorAccent(appliedColorAccent);

    onCloseHexColorPicker();

    if (appliedColorAccent === currentColorAccent) return;

    setChangeCurrentColorAccent(true);
  }, [appliedColorAccent, currentColorAccent]);

  const onAppliedColorButtons = useCallback(() => {
    setCurrentColorButtons(appliedColorButtons);

    onCloseHexColorPicker();

    if (appliedColorButtons === currentColorButtons) return;

    setChangeCurrentColorButtons(true);
  }, [appliedColorButtons]);

  const onSaveColorSchemeDialog = () => {
    // selectTheme.theme.accentColor = currentColorAccent;
    // selectTheme.theme.buttonsMain = currentColorButtons;

    const theme = {
      id: selectTheme.id,
      accentColor: currentColorAccent,
      buttonsMain: currentColorButtons,
      textColor: "#FFFFFF",
    };

    //setChangeTheme([...changeTheme, theme]);

    // onCloseColorSchemeDialog();
  };

  const nodeHexColorPickerButtons = viewMobile ? (
    <HexColorPickerComponent
      id="buttons-hex"
      onCloseHexColorPicker={onCloseHexColorPicker}
      onAppliedColor={onAppliedColorButtons}
      color={appliedColorButtons}
      setColor={setAppliedColorButtons}
      viewMobile={viewMobile}
    />
  ) : (
    <DropDownContainer
      directionX="right"
      manualY="62px"
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
          viewMobile={viewMobile}
        />
      </DropDownItem>
    </DropDownContainer>
  );

  const nodeHexColorPickerAccent = viewMobile ? (
    <HexColorPickerComponent
      id="accent-hex"
      onCloseHexColorPicker={onCloseHexColorPicker}
      onAppliedColor={onAppliedColorAccent}
      color={appliedColorAccent}
      setColor={setAppliedColorAccent}
      viewMobile={viewMobile}
    />
  ) : (
    <DropDownContainer
      directionX="right"
      manualY="62px"
      withBackdrop={false}
      isDefaultMode={false}
      open={openHexColorPickerAccent}
      clickOutsideAction={onCloseHexColorPicker}
      viewMobile={viewMobile}
    >
      <DropDownItem className="drop-down-item-hex">
        <HexColorPickerComponent
          id="accent-hex"
          onCloseHexColorPicker={onCloseHexColorPicker}
          onAppliedColor={onAppliedColorAccent}
          color={appliedColorAccent}
          setColor={setAppliedColorAccent}
          viewMobile={viewMobile}
        />
      </DropDownItem>
    </DropDownContainer>
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

  return viewMobile ? (
    <BreakpointWarning content={t("Settings:TheAppearanceSettings")} />
  ) : (
    <StyledComponent>
      <div className="header">{t("Common:Color")}</div>

      <div className="theme-standard">
        <div className="theme-name">{t("Common:Standard")}</div>

        <div className="theme-container">
          {appearanceTheme.map((item, index) => {
            return (
              <div
                key={index}
                id={item.id}
                style={{ background: item.accentColor }}
                className="box"
                onClick={() => onColorSelection(item)}
              >
                {onShowCheck(item.id)}
              </div>
            );
          })}

          {/* <div className="add-theme box" onClick={onAddTheme}>
          <img src="/static/images/plus.theme.svg" />
        </div> */}
        </div>
      </div>

      {/* <div onClick={onClickEdit}>Edit color scheme</div> */}
      <ColorSchemeDialog
        nodeButtonsColor={nodeButtonsColor}
        nodeAccentColor={nodeAccentColor}
        nodeHexColorPickerAccent={nodeHexColorPickerAccent}
        nodeHexColorPickerButtons={nodeHexColorPickerButtons}
        visible={showColorSchemeDialog}
        onClose={onCloseColorSchemeDialog}
        header={headerColorSchemeDialog}
        viewMobile={viewMobile}
        openHexColorPickerButtons={openHexColorPickerButtons}
        openHexColorPickerAccent={openHexColorPickerAccent}
        showRestoreToDefaultButtonDialog={showRestoreToDefaultButtonDialog}
        showSaveButtonDialog={showSaveButtonDialog}
        onSaveColorSchemeDialog={onSaveColorSchemeDialog}
      />
      <div className="header preview-header">{t("Common:Preview")}</div>
      <TabContainer elements={array_items} onSelect={onChangePreviewTheme} />

      <div className="buttons-container">
        <Button
          label="Save"
          onClick={onSaveSelectedColor}
          primary
          size="small"
        />
      </div>
    </StyledComponent>
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const {
    appearanceTheme,
    selectedThemeId,
    sendAppearanceTheme,
    getAppearanceTheme,
    currentColorScheme,
  } = settingsStore;

  return {
    appearanceTheme,
    selectedThemeId,
    sendAppearanceTheme,
    getAppearanceTheme,
    currentColorScheme,
  };
})(
  withTranslation(["Profile", "Common", "Settings"])(
    withRouter(observer(Appearance))
  )
);
