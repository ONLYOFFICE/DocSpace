import React, { useState, useEffect, useCallback, useMemo } from "react";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import withLoading from "SRC_DIR/HOCs/withLoading";
import globalColors from "@docspace/components/utils/globalColors";
import styled from "styled-components";
import TabContainer from "@docspace/components/tabs-container";
import Preview from "./settingsAppearance/preview";
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
`;

const Appearance = (props) => {
  const [selectedColor, setSelectedColor] = useState(1);

  const checkImg = <img src="/static/images/check.white.svg" />;

  const array_items = useMemo(
    () => [
      {
        key: "0",
        title: "Light theme",
        content: <Preview selectedColor={selectedColor} />,
      },
      {
        key: "1",
        title: "Dark theme",
        content: <Preview selectedColor={selectedColor} />,
      },
    ],
    [selectedColor]
  );

  useEffect(() => {}, [selectedColor]);

  const onColorSelection = (e) => {
    if (!e.target.id) return;

    const colorNumber = e.target.id[e.target.id.length - 1];
    setSelectedColor(+colorNumber);
  };

  const onShowCheck = (colorNumber) => {
    return selectedColor && selectedColor === colorNumber && checkImg;
  };

  return (
    <StyledComponent>
      <div>Color</div>
      <div>Header color is displayed only when light theme is applied</div>
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
      </div>
      <div>Preview</div>
      <TabContainer elements={array_items} />
      <Button
        label="Save"
        onClick={function noRefCheck() {}}
        primary
        size="small"
      />
    </StyledComponent>
  );
};

export default inject(({ auth, setup, common }) => {})(
  withLoading(withTranslation(["Settings", "Common"])(observer(Appearance)))
);
