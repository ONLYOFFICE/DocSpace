import CheckWhiteSvgUrl from "PUBLIC_DIR/images/check.white.svg?url";
import React, { useState, useEffect, useCallback, useMemo } from "react";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import Tooltip from "@docspace/components/tooltip";
import Text from "@docspace/components/text";
import TabContainer from "@docspace/components/tabs-container";
import Preview from "./Appearance/preview";
import { saveToSessionStorage, getFromSessionStorage } from "../../utils";
import ColorSchemeDialog from "./sub-components/colorSchemeDialog";

import DropDownItem from "@docspace/components/drop-down-item";
import DropDownContainer from "@docspace/components/drop-down";

import HexColorPickerComponent from "./sub-components/hexColorPicker";
import { isMobileOnly, isDesktop } from "react-device-detect";

import Loader from "./sub-components/loaderAppearance";

import { StyledComponent, StyledTheme } from "./Appearance/StyledApperance.js";
import { ReactSVG } from "react-svg";
import BreakpointWarning from "../../../../components/BreakpointWarning/index";
import ModalDialogDelete from "./sub-components/modalDialogDelete";
import hexToRgba from "hex-to-rgba";

const Appearance = (props) => {
  const {
    appearanceTheme,
    selectedThemeId,
    sendAppearanceTheme,
    getAppearanceTheme,
    currentColorScheme,
    deleteAppearanceTheme,
    tReady,
    t,
  } = props;

  const defaultAppliedColorAccent = currentColorScheme.main.accent;
  const defaultAppliedColorButtons = currentColorScheme.main.buttons;

  const headerAddTheme = t("Settings:NewColorScheme");
  const headerEditTheme = t("Settings:EditColorScheme");

  const checkImgHover = (
    <ReactSVG className="check-hover" src={CheckWhiteSvgUrl} />
  );

  const [showColorSchemeDialog, setShowColorSchemeDialog] = useState(false);

  const [headerColorSchemeDialog, setHeaderColorSchemeDialog] =
    useState(headerEditTheme);

  const [currentColorAccent, setCurrentColorAccent] = useState(null);
  const [currentColorButtons, setCurrentColorButtons] = useState(null);

  const [openHexColorPickerAccent, setOpenHexColorPickerAccent] =
    useState(false);
  const [openHexColorPickerButtons, setOpenHexColorPickerButtons] =
    useState(false);

  const [appliedColorAccent, setAppliedColorAccent] = useState(
    defaultAppliedColorAccent
  );
  const [appliedColorButtons, setAppliedColorButtons] = useState(
    defaultAppliedColorButtons
  );

  const [changeCurrentColorAccent, setChangeCurrentColorAccent] =
    useState(false);
  const [changeCurrentColorButtons, setChangeCurrentColorButtons] =
    useState(false);

  const [isSmallWindow, setIsSmallWindow] = useState(false);

  const [showSaveButtonDialog, setShowSaveButtonDialog] = useState(false);

  const [isEditDialog, setIsEditDialog] = useState(false);
  const [isAddThemeDialog, setIsAddThemeDialog] = useState(false);

  const [previewAccent, setPreviewAccent] = useState(
    currentColorScheme.main.accent
  );

  const [colorCheckImg, setColorCheckImg] = useState(
    currentColorScheme.text.accent
  );
  const [colorCheckImgHover, setColorCheckImgHover] = useState(
    currentColorScheme.text.accent
  );

  const [selectThemeId, setSelectThemeId] = useState(selectedThemeId);

  const [isDisabledSaveButton, setIsDisabledSaveButton] = useState(true);

  const [abilityAddTheme, setAbilityAddTheme] = useState(true);

  const [isDisabledEditButton, setIsDisabledEditButton] = useState(true);
  const [isDisabledDeleteButton, setIsDisabledDeleteButton] = useState(true);
  const [isShowDeleteButton, setIsShowDeleteButton] = useState(false);

  const [visibleDialog, setVisibleDialog] = useState(false);

  const [theme, setTheme] = useState(appearanceTheme);

  const array_items = useMemo(
    () => [
      {
        id: "light-theme",
        key: "0",
        title: t("Profile:LightTheme"),
        content: (
          <Preview
            appliedColorAccent={appliedColorAccent}
            previewAccent={previewAccent}
            selectThemeId={selectThemeId}
            colorCheckImg={colorCheckImg}
            themePreview="Light"
          />
        ),
      },
      {
        id: "dark-theme",
        key: "1",
        title: t("Profile:DarkTheme"),
        content: (
          <Preview
            appliedColorAccent={appliedColorAccent}
            previewAccent={previewAccent}
            selectThemeId={selectThemeId}
            colorCheckImg={colorCheckImg}
            themePreview="Dark"
          />
        ),
      },
    ],
    [previewAccent, selectThemeId, colorCheckImg, tReady]
  );

  const getSettings = () => {
    const selectColorId = getFromSessionStorage("selectColorId");
    const defaultColorId = selectedThemeId;
    saveToSessionStorage("defaultColorId", defaultColorId);
    if (selectColorId) {
      setSelectThemeId(selectColorId);
    } else {
      setSelectThemeId(defaultColorId);
    }
  };

  useEffect(() => {
    getSettings();
  }, []);

  useEffect(() => {
    saveToSessionStorage("selectColorId", selectThemeId);
  }, [selectThemeId]);

  useEffect(() => {
    onCheckView();
    window.addEventListener("resize", onCheckView);

    return () => {
      window.removeEventListener("resize", onCheckView);
    };
  }, []);

  useEffect(() => {
    if (!currentColorScheme) return;

    setAppliedColorButtons(defaultAppliedColorButtons);
    setAppliedColorAccent(defaultAppliedColorAccent);
  }, [
    currentColorScheme,
    defaultAppliedColorButtons,
    defaultAppliedColorAccent,
  ]);

  useEffect(() => {
    onColorCheck(appearanceTheme);

    // Setting a checkbox for a new theme
    setTheme(appearanceTheme);
    if (appearanceTheme.length > theme.length) {
      const newTheme = appearanceTheme[appearanceTheme.length - 1];
      const idNewTheme = newTheme.id;
      const accentNewTheme = newTheme.main.accent;

      setSelectThemeId(idNewTheme);
      setPreviewAccent(accentNewTheme);
    }

    if (appearanceTheme.length === 9) {
      setAbilityAddTheme(false);
    } else {
      setAbilityAddTheme(true);
    }

    if (appearanceTheme.length === 6) {
      setIsShowDeleteButton(false);
    } else {
      setIsShowDeleteButton(true);
    }
  }, [
    appearanceTheme,
    theme,
    setSelectThemeId,
    setPreviewAccent,
    setAbilityAddTheme,
    setIsShowDeleteButton,
  ]);

  useEffect(() => {
    onColorCheck(appearanceTheme);

    if (appearanceTheme.find((theme) => theme.id == selectThemeId).name) {
      setIsDisabledEditButton(true);
      setIsDisabledDeleteButton(true);
      return;
    }

    setIsDisabledEditButton(false);
    setIsDisabledDeleteButton(false);
  }, [selectThemeId]);

  useEffect(() => {
    if (selectThemeId === selectedThemeId) {
      setIsDisabledSaveButton(true);
    } else {
      setIsDisabledSaveButton(false);
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

    if (
      !changeCurrentColorAccent &&
      !changeCurrentColorButtons &&
      isEditDialog
    ) {
      setShowSaveButtonDialog(false);
    }
  }, [
    selectedThemeId,
    selectThemeId,
    changeCurrentColorAccent,
    changeCurrentColorButtons,
    isAddThemeDialog,
    isEditDialog,
    previewAccent,
  ]);

  const onColorCheck = useCallback(
    (themes) => {
      const colorCheckImg = themes.find((theme) => theme.id == selectThemeId)
        ?.text.accent;

      setColorCheckImg(colorCheckImg);
    },
    [selectThemeId]
  );

  const onColorCheckImgHover = useCallback(
    (e) => {
      const id = e.target.id;
      if (!id) return;

      const colorCheckImg = appearanceTheme.find((theme) => theme.id == id).text
        .accent;

      setColorCheckImgHover(colorCheckImg);
    },
    [appearanceTheme]
  );

  const onCheckView = () => {
    if (isDesktop && window.innerWidth < 600) {
      setIsSmallWindow(true);
    } else {
      setIsSmallWindow(false);
    }
  };

  const onColorSelection = useCallback(
    (e) => {
      const theme = e.currentTarget;
      const id = +theme.id;
      const accent = appearanceTheme.find((theme) => theme.id == id).main
        .accent;

      setPreviewAccent(accent);
      setSelectThemeId(id);
      saveToSessionStorage("selectColorId", id);
      saveToSessionStorage("selectColorAccent", accent);
    },
    [appearanceTheme, setPreviewAccent, setSelectThemeId]
  );

  const onSave = useCallback(async () => {
    setIsDisabledSaveButton(true);

    if (!selectThemeId) return;

    try {
      await sendAppearanceTheme({ selected: selectThemeId });
      await getAppearanceTheme();
      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
    } catch (error) {
      toastr.error(error);
    }
    saveToSessionStorage("selectColorId", selectThemeId);
    saveToSessionStorage("defaultColorId", selectThemeId);
    saveToSessionStorage("selectColorAccent", previewAccent);
    saveToSessionStorage("defaultColorAccent", previewAccent);
  }, [
    selectThemeId,
    setIsDisabledSaveButton,
    sendAppearanceTheme,
    getAppearanceTheme,
  ]);

  // Open HexColorPicker
  const onClickColor = (e) => {
    if (e.target.id === "accent") {
      setOpenHexColorPickerAccent(true);
      setOpenHexColorPickerButtons(false);
    } else {
      setOpenHexColorPickerButtons(true);
      setOpenHexColorPickerAccent(false);
    }
  };

  const onClickDeleteModal = useCallback(async () => {
    try {
      await deleteAppearanceTheme(selectThemeId);
      await getAppearanceTheme();

      if (selectedThemeId !== selectThemeId) {
        setSelectThemeId(selectedThemeId);
        setPreviewAccent(currentColorScheme.main.accent);
      }

      if (selectedThemeId === selectThemeId) {
        setSelectThemeId(appearanceTheme[0].id);
        setPreviewAccent(appearanceTheme[0].main.accent);
      }

      saveToSessionStorage("selectColorId", appearanceTheme[0].id);
      saveToSessionStorage("selectColorAccent", appearanceTheme[0].main.accent);

      onCloseDialogDelete();

      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
    } catch (error) {
      toastr.error(error);
    }
  }, [
    selectThemeId,
    selectedThemeId,
    onCloseDialogDelete,
    deleteAppearanceTheme,
    getAppearanceTheme,
  ]);

  const onCloseColorSchemeDialog = () => {
    setShowColorSchemeDialog(false);

    setOpenHexColorPickerAccent(false);
    setOpenHexColorPickerButtons(false);

    setChangeCurrentColorAccent(false);
    setChangeCurrentColorButtons(false);

    setIsEditDialog(false);
    setIsAddThemeDialog(false);

    setShowSaveButtonDialog(false);

    setCurrentColorAccent(null);
    setCurrentColorButtons(null);

    setAppliedColorAccent(defaultAppliedColorAccent);
    setAppliedColorButtons(defaultAppliedColorButtons);
  };

  const onAddTheme = () => {
    if (!abilityAddTheme) return;
    setIsAddThemeDialog(true);

    setHeaderColorSchemeDialog(headerAddTheme);

    setShowColorSchemeDialog(true);
  };

  const onClickEdit = () => {
    appearanceTheme.map((item) => {
      if (item.id === selectThemeId) {
        setCurrentColorAccent(item.main.accent.toUpperCase());
        setCurrentColorButtons(item.main.buttons.toUpperCase());

        setAppliedColorAccent(item.main.accent.toUpperCase());
        setAppliedColorButtons(item.main.buttons.toUpperCase());
      }
    });

    setIsEditDialog(true);

    setHeaderColorSchemeDialog(headerEditTheme);

    setShowColorSchemeDialog(true);
  };

  const onCloseHexColorPickerAccent = useCallback(() => {
    setOpenHexColorPickerAccent(false);
    if (!currentColorAccent) return;
    setAppliedColorAccent(currentColorAccent);
  }, [currentColorAccent, setOpenHexColorPickerAccent, setAppliedColorAccent]);

  const onCloseHexColorPickerButtons = useCallback(() => {
    setOpenHexColorPickerButtons(false);
    if (!currentColorButtons) return;
    setAppliedColorButtons(currentColorButtons);
  }, [
    currentColorButtons,
    setOpenHexColorPickerButtons,
    setAppliedColorButtons,
  ]);

  const getTextColor = (color) => {
    const black = "#333333";
    const white = "#FFFFFF";

    const rgba = hexToRgba(color)
      .replace("rgba(", "")
      .replace(")", "")
      .split(", ");

    const r = rgba[0];
    const g = rgba[1];
    const b = rgba[2];

    const textColor =
      (r * 299 + g * 587 + b * 114) / 1000 > 128 ? black : white;

    return textColor;
  };

  const onAppliedColorAccent = useCallback(() => {
    if (appliedColorAccent.toUpperCase() !== currentColorAccent) {
      setChangeCurrentColorAccent(true);
    }

    setCurrentColorAccent(appliedColorAccent);
    saveToSessionStorage("selectColorAccent", appliedColorAccent);

    setOpenHexColorPickerAccent(false);
  }, [
    appliedColorAccent,
    currentColorAccent,
    setChangeCurrentColorAccent,
    setOpenHexColorPickerAccent,
  ]);

  const onAppliedColorButtons = useCallback(() => {
    if (appliedColorButtons.toUpperCase() !== currentColorButtons) {
      setChangeCurrentColorButtons(true);
    }

    setCurrentColorButtons(appliedColorButtons);

    setOpenHexColorPickerButtons(false);
  }, [
    appliedColorButtons,
    currentColorButtons,
    setChangeCurrentColorButtons,
    setOpenHexColorPickerButtons,
  ]);

  const onSaveNewThemes = useCallback(
    async (theme) => {
      try {
        await sendAppearanceTheme({ theme: theme });
        await getAppearanceTheme();

        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
      } catch (error) {
        toastr.error(error);
      }
    },
    [sendAppearanceTheme, getAppearanceTheme]
  );

  const onSaveChangedThemes = useCallback(
    async (editTheme) => {
      try {
        await sendAppearanceTheme({ theme: editTheme });
        await getAppearanceTheme();
        setPreviewAccent(editTheme.main.accent);

        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
      } catch (error) {
        toastr.error(error);
      }
    },
    [sendAppearanceTheme, getAppearanceTheme]
  );

  const onSaveColorSchemeDialog = () => {
    const textColorAccent = getTextColor(currentColorAccent);
    const textColorButtons = getTextColor(currentColorButtons);

    if (isAddThemeDialog) {
      // Saving a new custom theme
      const theme = {
        main: {
          accent: currentColorAccent,
          buttons: currentColorButtons,
        },
        text: {
          accent: textColorAccent,
          buttons: textColorButtons,
        },
      };

      onSaveNewThemes(theme);

      setCurrentColorAccent(null);
      setCurrentColorButtons(null);

      onCloseColorSchemeDialog();

      return;
    }

    // Editing themes
    const editTheme = {
      id: selectThemeId,
      main: {
        accent: currentColorAccent,
        buttons: currentColorButtons,
      },
      text: {
        accent: textColorAccent,
        buttons: textColorButtons,
      },
    };

    onSaveChangedThemes(editTheme);

    setCurrentColorAccent(appliedColorAccent);
    setCurrentColorButtons(appliedColorButtons);

    onCloseColorSchemeDialog();
  };

  const onCloseDialogDelete = () => {
    setVisibleDialog(false);
  };

  const onOpenDialogDelete = () => {
    setVisibleDialog(true);
  };

  const nodeHexColorPickerButtons = (
    <DropDownContainer
      directionX="right"
      manualY="62px"
      withBackdrop={false}
      isDefaultMode={false}
      open={openHexColorPickerButtons}
      clickOutsideAction={onCloseHexColorPickerButtons}
    >
      <DropDownItem className="drop-down-item-hex">
        <HexColorPickerComponent
          id="buttons-hex"
          onCloseHexColorPicker={onCloseHexColorPickerButtons}
          onAppliedColor={onAppliedColorButtons}
          color={appliedColorButtons}
          setColor={setAppliedColorButtons}
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
      clickOutsideAction={onCloseHexColorPickerAccent}
    >
      <DropDownItem className="drop-down-item-hex">
        <HexColorPickerComponent
          id="accent-hex"
          onCloseHexColorPicker={onCloseHexColorPickerAccent}
          onAppliedColor={onAppliedColorAccent}
          color={appliedColorAccent}
          setColor={setAppliedColorAccent}
        />
      </DropDownItem>
    </DropDownContainer>
  );

  const textTooltip = () => {
    return (
      <Text fontSize="12px" noSelect>
        {t("Settings:LimitThemesTooltip")}
      </Text>
    );
  };

  if (isSmallWindow)
    return (
      <BreakpointWarning sectionName={t("Settings:Appearance")} isSmallWindow />
    );
  if (isMobileOnly)
    return <BreakpointWarning sectionName={t("Settings:Appearance")} />;

  return !tReady ? (
    <Loader />
  ) : (
    <>
      <ModalDialogDelete
        visible={visibleDialog}
        onClose={onCloseDialogDelete}
        onClickDelete={onClickDeleteModal}
      />

      <StyledComponent colorCheckImg={colorCheckImg}>
        <div className="header">{t("Common:Color")}</div>

        <div className="theme-standard-container">
          <div className="theme-name">{t("Common:Standard")}</div>

          <div className="theme-container">
            {appearanceTheme.map((item, index) => {
              if (!item.name) return;
              return (
                <StyledTheme
                  key={index}
                  id={item.id}
                  colorCheckImgHover={colorCheckImgHover}
                  style={{ background: item.main.accent }}
                  onClick={onColorSelection}
                  onMouseOver={onColorCheckImgHover}
                >
                  {selectThemeId === item.id && (
                    <ReactSVG className="check-img" src={CheckWhiteSvgUrl} />
                  )}

                  {selectThemeId !== item.id && checkImgHover}
                </StyledTheme>
              );
            })}
          </div>
        </div>

        <div className="theme-custom-container">
          <div className="theme-name">{t("Settings:Custom")}</div>

          <div className="theme-container">
            <div className="custom-themes">
              {appearanceTheme.map((item, index) => {
                if (item.name) return;
                return (
                  <StyledTheme
                    key={index}
                    id={item.id}
                    style={{ background: item.main.accent }}
                    colorCheckImgHover={colorCheckImgHover}
                    onClick={onColorSelection}
                    onMouseOver={onColorCheckImgHover}
                  >
                    {selectThemeId === item.id && (
                      <ReactSVG className="check-img" src={CheckWhiteSvgUrl} />
                    )}
                    {selectThemeId !== item.id && checkImgHover}
                  </StyledTheme>
                );
              })}
            </div>

            <div
              data-for="theme-add"
              data-tip="tooltip"
              className="theme-add"
              onClick={onAddTheme}
            />
            {!abilityAddTheme && (
              <Tooltip
                id="theme-add"
                offsetBottom={0}
                offsetRight={130}
                effect="solid"
                place="bottom"
                getContent={textTooltip}
                maxWidth="300px"
              />
            )}
          </div>
        </div>

        <ColorSchemeDialog
          onClickColor={onClickColor}
          currentColorAccent={currentColorAccent}
          currentColorButtons={currentColorButtons}
          nodeHexColorPickerAccent={nodeHexColorPickerAccent}
          nodeHexColorPickerButtons={nodeHexColorPickerButtons}
          visible={showColorSchemeDialog}
          onClose={onCloseColorSchemeDialog}
          header={headerColorSchemeDialog}
          viewMobile={isMobileOnly}
          openHexColorPickerButtons={openHexColorPickerButtons}
          openHexColorPickerAccent={openHexColorPickerAccent}
          showSaveButtonDialog={showSaveButtonDialog}
          onSaveColorSchemeDialog={onSaveColorSchemeDialog}
        />
        <div className="header preview-header">{t("Common:Preview")}</div>
        <TabContainer elements={array_items} />

        <div className="buttons-container">
          <Button
            className="save button"
            label={t("Common:SaveButton")}
            onClick={onSave}
            primary
            size="small"
            isDisabled={isDisabledSaveButton}
          />

          <Button
            className="edit-current-theme button"
            label={t("Settings:EditCurrentTheme")}
            onClick={onClickEdit}
            size="small"
            isDisabled={isDisabledEditButton}
          />
          {isShowDeleteButton && (
            <Button
              className="delete-theme button"
              label={t("Settings:DeleteTheme")}
              onClick={onOpenDialogDelete}
              size="small"
              isDisabled={isDisabledDeleteButton}
            />
          )}
        </div>
      </StyledComponent>
    </>
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
    deleteAppearanceTheme,
    theme,
  } = settingsStore;

  return {
    appearanceTheme,
    selectedThemeId,
    sendAppearanceTheme,
    getAppearanceTheme,
    currentColorScheme,
    deleteAppearanceTheme,
    theme,
  };
})(withTranslation(["Profile", "Common", "Settings"])(observer(Appearance)));
