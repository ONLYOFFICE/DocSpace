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
  /* #color-scheme_1 {
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
  } */

  .add-theme {
    background: #d0d5da;
    padding-top: 16px;
    padding-left: 16px;
    box-sizing: border-box;
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

  const arrayTheme = [
    {
      id: 1,
      theme: {
        accentColor: "#4781D1",
        buttonsMain: "#5299E0",
        textColor: "#FFFFFF",
      },
      isSelected: true,
    },
    {
      id: 2,
      theme: {
        accentColor: "#F97A0B",
        buttonsMain: "#FF9933",
        textColor: "#FFFFFF",
      },
      isSelected: false,
    },
    {
      id: 3,
      theme: {
        accentColor: "#2DB482",
        buttonsMain: "#22C386",
        textColor: "#FFFFFF",
      },
      isSelected: false,
    },
    {
      id: 4,
      theme: {
        accentColor: "#F2675A",
        buttonsMain: "#F27564",
        textColor: "#FFFFFF",
      },
      isSelected: false,
    },
    {
      id: 5,
      theme: {
        accentColor: "#6D4EC2",
        buttonsMain: "#8570BD",
        textColor: "#FFFFFF",
      },
      isSelected: false,
    },
    {
      id: 6,
      theme: {
        accentColor: "#11A4D4",
        buttonsMain: "#13B7EC",
        textColor: "#FFFFFF",
      },
      isSelected: false,
    },
    {
      id: 7,
      theme: {
        accentColor: "#444444",
        buttonsMain: "#6E6E6E",
        textColor: "#FFFFFF",
      },
      isSelected: false,
    },
  ];

  const [selectTheme, setSelectTheme] = useState({
    id: 1,
    theme: {
      accentColor: "#4781D1",
      buttonsMain: "#5299E0",
      textColor: "#FFFFFF",
    },
    isSelected: true,
  });

  const [changeTheme, setChangeTheme] = useState([]);

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
    if (isMobileOnly || window.innerWidth <= 428) {
      setViewMobile(true);
    } else {
      setViewMobile(false);
    }
  };

  const onColorSelection = (e) => {
    if (!e.target.id) return;

    const colorNumber = +e.target.id;

    console.log("e.target.id", e.target.id);
    setSelectedColor(colorNumber);

    //TODO: find id and give item

    //TODO: if time array = 0, then arrayTheme, else time array
    const theme = arrayTheme.find((item) => item.id === colorNumber);
    theme.isSelected = true;
    console.log("theme", theme);

    setSelectTheme(theme);
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
    arrayTheme.map((item) => {
      if (item.id === selectedColor) {
        // TODO: give store Accent color and Buttons main to id

        setCurrentColorAccent(item.theme.accentColor);
        setCurrentColorButtons(item.theme.buttonsMain);
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
    selectTheme.theme.accentColor = currentColorAccent;
    selectTheme.theme.buttonsMain = currentColorButtons;

    setChangeTheme([...changeTheme, selectTheme]);

    onCloseColorSchemeDialog();
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

  console.log(
    "currentColorAccent,currentColorButtons,selectTheme,changeTheme ",
    currentColorAccent,
    currentColorButtons,
    selectTheme,
    changeTheme
  );

  return (
    <StyledComponent>
      <div>Color</div>

      <div className="container">
        {arrayTheme.map((item) => {
          return (
            <div
              id={item.id}
              style={{ background: item.theme.accentColor }}
              className="box"
              onClick={onColorSelection}
            >
              {onShowCheck(item.id)}
            </div>
          );
        })}
        {/* <div id="color-scheme_1" className="box" onClick={onColorSelection}>
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
        </div> */}
        {/* <div className="add-theme box" onClick={onAddTheme}>
          <img src="/static/images/plus.theme.svg" />
        </div> */}
      </div>

      <div onClick={onClickEdit}>Edit color scheme</div>
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
