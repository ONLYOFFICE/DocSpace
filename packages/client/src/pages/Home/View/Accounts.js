import React from "react";
import { Consumer } from "@docspace/components/utils/context";

import { AccountsSectionBodyContent } from "../Section";

const AccountsView = () => {
  return (
    <Consumer>
      {(context) => (
        <>
          <AccountsSectionBodyContent sectionWidth={context.sectionWidth} />
        </>
      )}
    </Consumer>
  );
};

export default AccountsView;
