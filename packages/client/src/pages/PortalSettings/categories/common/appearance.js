import React, { useState, useEffect, useCallback, useMemo } from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import toastr from "@docspace/components/toast/toastr";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import Tooltip from "@docspace/components/tooltip";
import Text from "@docspace/components/text";
import TabContainer from "@docspace/components/tabs-container";
import Preview from "./Appearance/preview";

import ColorSchemeDialog from "./sub-components/colorSchemeDialog";

import DropDownItem from "@docspace/components/drop-down-item";
import DropDownContainer from "@docspace/components/drop-down";

import HexColorPickerComponent from "./sub-components/hexColorPicker";
import { isMobileOnly } from "react-device-detect";

import Loader from "./sub-components/loaderAppearance";

import { StyledComponent } from "./Appearance/StyledApperance.js";

import BreakpointWarning from "../../../../components/BreakpointWarning/index";

const Appearance = (props) => {
  const {
    appearanceTheme,
    selectedThemeId,
    sendAppearanceTheme,
    getAppearanceTheme,
    currentColorScheme,
    onSaveSelectedNewThemes,
    deleteAppearanceTheme,
    tReady,
    t,
  } = props;

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

  const [previewAccentColor, setPreviewAccentColor] = useState(
    currentColorScheme.accentColor
  );
  const [selectThemeId, setSelectThemeId] = useState(selectedThemeId);
  const [selectNewThemeId, setSelectNewThemeId] = useState(null);

  const [isDisabledSaveButton, setIsDisabledSaveButton] = useState(true);

  const [newCustomThemes, setNewCustomThemes] = useState([]);
  const [changeThemes, setChangeThemes] = useState([]);
  const [editAppearanceTheme, setEditAppearanceTheme] = useState([]);

  const [abilityAddTheme, setAbilityAddTheme] = useState(true);

  const [isDisabledEditButton, setIsDisabledEditButton] = useState(true);
  const [isDisabledDeleteButton, setIsDisabledDeleteButton] = useState(true);

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
            previewAccentColor={previewAccentColor}
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
            previewAccentColor={previewAccentColor}
            selectThemeId={selectThemeId}
            themePreview="Dark"
          />
        ),
      },
    ],
    [previewAccentColor, previewTheme, selectThemeId, tReady]
  );

  useEffect(() => {
    const numberThemes = newCustomThemes.length + appearanceTheme.length;
    if (numberThemes === 10) {
      setAbilityAddTheme(false);
    } else {
      setAbilityAddTheme(true);
    }
  }, [newCustomThemes.length, appearanceTheme.length, setAbilityAddTheme]);

  useEffect(() => {
    onCheckView();
    window.addEventListener("resize", onCheckView);

    return () => window.removeEventListener("resize", onCheckView);
  }, []);

  useEffect(() => {
    setSelectThemeId(selectedThemeId);
  }, [selectedThemeId]);

  useEffect(() => {
    if (selectThemeId < 8) {
      setIsDisabledEditButton(false);
      setIsDisabledDeleteButton(true);
      return;
    }

    setIsDisabledEditButton(true);
    setIsDisabledDeleteButton(false);
  }, [selectThemeId]);

  useEffect(() => {
    if (
      previewAccentColor !== currentColorScheme.accentColor ||
      newCustomThemes.length > 0
    ) {
      setIsDisabledSaveButton(false);
    } else {
      setIsDisabledSaveButton(true);
    }

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
    previewAccentColor,
    newCustomThemes.length,
  ]);

  const onCheckView = () => {
    if (isMobileOnly || window.innerWidth < 600) {
      setViewMobile(true);
    } else {
      setViewMobile(false);
    }
  };

  const onColorSelection = (item) => {
    setPreviewAccentColor(item.accentColor);
    setSelectThemeId(item.id);
    setSelectNewThemeId(null);
  };

  const onColorSelectionNewThemes = (item, index) => {
    setPreviewAccentColor(item.accentColor);
    setSelectNewThemeId(index);
    setSelectThemeId(null);
  };

  const onShowCheck = useCallback(
    (colorNumber) => {
      return selectThemeId && selectThemeId === colorNumber && checkImg;
    },
    [selectThemeId, checkImg]
  );

  const onShowCheckNewThemes = useCallback(
    (colorNumber) => {
      return selectNewThemeId && selectNewThemeId === colorNumber && checkImg;
    },
    [selectNewThemeId, checkImg]
  );

  const onChangePreviewTheme = (e) => {
    setPreviewTheme(e.title);
  };

  const onSaveNewThemes = useCallback(async () => {
    try {
      await sendAppearanceTheme({ themes: newCustomThemes });
      await getAppearanceTheme();
      setNewCustomThemes([]);

      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
    } catch (error) {
      toastr.error(error);
    }
  }, [newCustomThemes.length, sendAppearanceTheme, getAppearanceTheme]);

  const onSaveSelectedThemes = useCallback(async () => {
    try {
      await sendAppearanceTheme({ selected: selectThemeId });
      await getAppearanceTheme();

      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
    } catch (error) {
      toastr.error(error);
    }
  }, [selectThemeId, sendAppearanceTheme, getAppearanceTheme]);

  const onSaveChangedThemes = useCallback(async () => {
    try {
      await sendAppearanceTheme({ themes: changeThemes });
      await getAppearanceTheme();

      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));

      setChangeThemes([]);
    } catch (error) {
      toastr.error(error);
    }
  }, [
    changeThemes.length,
    setChangeThemes,
    sendAppearanceTheme,
    getAppearanceTheme,
  ]);

  const onSaveSelectedChangedThemes = useCallback(async () => {
    try {
      await sendAppearanceTheme({ themes: changeThemes });
      await sendAppearanceTheme({ selected: selectThemeId });
      await getAppearanceTheme();

      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));

      setChangeThemes([]);
    } catch (error) {
      toastr.error(error);
    }
  }, [
    changeThemes.length,
    setChangeThemes,
    sendAppearanceTheme,
    getAppearanceTheme,
  ]);

  const onSave = useCallback(async () => {
    setIsDisabledSaveButton(true);

    if (
      newCustomThemes.length === 0 &&
      !selectNewThemeId &&
      !selectThemeId &&
      changeThemes.length === 0
    ) {
      return;
    }

    // Saving the modified and selected theme
    if (changeThemes.length > 0 && selectThemeId) {
      onSaveSelectedChangedThemes();

      return;
    }

    // Saving a modified theme
    if (changeThemes.length > 0 && !selectThemeId) {
      onSaveChangedThemes();

      return;
    }

    if (newCustomThemes.length > 0 && selectNewThemeId) {
      try {
        await onSaveSelectedNewThemes(
          newCustomThemes,
          selectNewThemeId,
          appearanceTheme.length
        );

        setNewCustomThemes([]);
        setSelectNewThemeId(null);
      } catch (error) {
        toastr.error(error);
      }

      return;
    }

    if (newCustomThemes.length > 0) {
      onSaveNewThemes();

      return;
    }

    if (selectThemeId) {
      onSaveSelectedThemes();
      return;
    }
  }, [
    changeThemes.length,
    newCustomThemes.length,
    appearanceTheme.length,
    selectNewThemeId,
    selectThemeId,
    onSaveSelectedNewThemes,
    onSaveNewThemes,
    onSaveSelectedThemes,
    getAppearanceTheme,
  ]);

  const onClickEdit = () => {
    appearanceTheme.map((item) => {
      if (item.id === selectThemeId) {
        setCurrentColorAccent(item.accentColor);
        setCurrentColorButtons(item.buttonsMain);
      }
    });

    setIsEditDialog(true);

    setHeaderColorSchemeDialog("Edit color scheme");

    // setShowRestoreToDefaultButtonDialog(true);

    setShowColorSchemeDialog(true);
  };

  const onClickDelete = useCallback(async () => {
    try {
      await deleteAppearanceTheme(selectThemeId);
      await getAppearanceTheme();

      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
    } catch (error) {
      toastr.error(error);
    }
  }, [selectThemeId, deleteAppearanceTheme, getAppearanceTheme]);

  const onCloseColorSchemeDialog = () => {
    setShowColorSchemeDialog(false);

    setOpenHexColorPickerAccent(false);
    setOpenHexColorPickerButtons(false);

    setChangeCurrentColorAccent(false);
    setChangeCurrentColorButtons(false);

    setIsEditDialog(false);

    setShowSaveButtonDialog(false);
  };

  const onAddTheme = () => {
    if (!abilityAddTheme) return;
    setIsAddThemeDialog(true);
    // setCurrentColorAccent(
    //   "url(/static/images/plus.theme.svg) 15px 15px no-repeat #D0D5DA"
    // );
    // setCurrentColorButtons(
    //   "url(/static/images/plus.theme.svg) 15px 15px no-repeat #D0D5DA"
    // );

    setCurrentColorAccent(null);
    setCurrentColorButtons(null);

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
    if (isAddThemeDialog) {
      // Saving a new custom theme
      const theme = {
        accentColor: currentColorAccent,
        buttonsMain: currentColorButtons,
        textColor: "#FFFFFF",
      };

      setNewCustomThemes([...newCustomThemes, theme]);
      setCurrentColorAccent(null);
      setCurrentColorButtons(null);

      onCloseColorSchemeDialog();
      setIsAddThemeDialog(false);

      return;
    }

    // Editing all themes
    const editTheme = {
      id: selectThemeId,
      accentColor: currentColorAccent,
      buttonsMain: currentColorButtons,
      textColor: "#FFFFFF",
    };

    const editAppearanceTheme = appearanceTheme.map((theme) => {
      if (theme.id === selectThemeId) {
        return {
          id: selectThemeId,
          accentColor: currentColorAccent,
          buttonsMain: currentColorButtons,
          textColor: "#FFFFFF",
        };
      }
      return theme;
    });

    setChangeThemes([...changeThemes, editTheme]);
    setEditAppearanceTheme(editAppearanceTheme);

    setPreviewAccentColor(currentColorAccent);

    onCloseColorSchemeDialog();
  };

  const nodeHexColorPickerButtons = (
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

  const nodeHexColorPickerAccent = (
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

  // const nodeAccentColor = (
  //   <div
  //     id="accent"
  //     style={{ background: currentColorAccent }}
  //     className="color-button"
  //     onClick={onClickColor}
  //   ></div>
  // );

  // const nodeButtonsColor = (
  //   <div
  //     id="buttons"
  //     style={{ background: currentColorButtons }}
  //     className="color-button"
  //     onClick={onClickColor}
  //   ></div>
  // );

  return viewMobile ? (
    <BreakpointWarning sectionName={t("Settings:Appearance")} />
  ) : !tReady ? (
    <Loader />
  ) : (
    <StyledComponent>
      <div className="header">{t("Common:Color")}</div>

      <div className="theme-standard">
        <div className="theme-name">{t("Common:Standard")}</div>

        <div className="theme-container">
          {(editAppearanceTheme.length === 0
            ? appearanceTheme
            : editAppearanceTheme
          ).map((item, index) => {
            if (index > 6) return;
            return (
              <div
                key={index}
                id={item.id}
                style={{ background: item.accentColor }}
                className="box"
                onClick={() => onColorSelection(item)}
              >
                {!selectNewThemeId && onShowCheck(item.id)}
              </div>
            );
          })}
        </div>
      </div>

      <div className="theme-custom">
        <div className="theme-name">Custom</div>

        <div className="theme-container">
          {(editAppearanceTheme.length === 0
            ? appearanceTheme
            : editAppearanceTheme
          ).map((item, index) => {
            if (index < 7) return;
            return (
              <div
                key={index}
                id={item.id}
                style={{ background: item.accentColor }}
                className="box"
                onClick={() => onColorSelection(item)}
              >
                {!selectNewThemeId && onShowCheck(item.id)}
              </div>
            );
          })}

          {newCustomThemes?.map((item, index) => {
            return (
              <div
                key={index}
                //  id={item.id}
                style={{ background: item.accentColor }}
                className="box"
                onClick={() => onColorSelectionNewThemes(item, index + 1)}
              >
                {!selectThemeId && onShowCheckNewThemes(index + 1)}
              </div>
            );
          })}

          <div
            data-for="theme-add"
            data-tip={
              !abilityAddTheme
                ? "You can only create 3 custom themes. To create a new one, you must delete one of the previous themes."
                : null
            }
            className="box theme-add"
            onClick={() => onAddTheme()}
          />
          <Tooltip
            id="theme-add"
            offsetBottom={0}
            effect="solid"
            place="bottom"
            getContent={(dataTip) => (
              <Text fontSize="12px" noSelect>
                {dataTip}
              </Text>
            )}
          ></Tooltip>
        </div>
      </div>

      <ColorSchemeDialog
        // nodeButtonsColor={nodeButtonsColor}
        // nodeAccentColor={nodeAccentColor}

        onClickColor={onClickColor}
        currentColorAccent={currentColorAccent}
        currentColorButtons={currentColorButtons}
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
          className="button"
          label="Save"
          onClick={onSave}
          primary
          size="small"
          isDisabled={isDisabledSaveButton}
        />

        <Button
          className="button"
          label="Edit current theme"
          onClick={onClickEdit}
          size="small"
          // isDisabled={isDisabledEditButton}
        />
        <Button
          className="button"
          label="Delete theme"
          onClick={onClickDelete}
          size="small"
          isDisabled={isDisabledDeleteButton}
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
    onSaveSelectedNewThemes,
    deleteAppearanceTheme,
  } = settingsStore;

  return {
    appearanceTheme,
    selectedThemeId,
    sendAppearanceTheme,
    getAppearanceTheme,
    currentColorScheme,
    onSaveSelectedNewThemes,
    deleteAppearanceTheme,
  };
})(
  withTranslation(["Profile", "Common", "Settings"])(
    withRouter(observer(Appearance))
  )
);
