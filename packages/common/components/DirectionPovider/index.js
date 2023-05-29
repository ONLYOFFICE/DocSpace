import React from "react";
import DirectionContext from "../../contexts/DirectionContext";
import { inject, observer } from "mobx-react";

const DirectionProvider = ({ interfaceDirection, children }) => {
  return (
    <DirectionContext.Provider value={interfaceDirection}>
      {children}
    </DirectionContext.Provider>
  );
};

export default inject(({ auth }) => {
  const { interfaceDirection } = auth.settingsStore;

  return { interfaceDirection };
})(observer(DirectionProvider));
