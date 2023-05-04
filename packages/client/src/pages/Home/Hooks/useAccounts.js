import React from "react";

import AccountsFilter from "@docspace/common/api/people/filter";

import { setDocumentTitle } from "SRC_DIR/helpers/utils";

const useAccounts = ({
  t,
  isAccountsPage,
  location,
  setIsLoading,
  clearFiles,
  setSelectedNode,
  fetchPeople,
  setPortalTariff,
}) => {
  React.useEffect(() => {
    if (!isAccountsPage) return;
    setIsLoading(true);
    setSelectedNode(["accounts", "filter"]);
    clearFiles();
    const newFilter = AccountsFilter.getFilter(location);

    setDocumentTitle(t("Common:Accounts"));

    fetchPeople(newFilter, true)
      .catch((err) => {
        if (err?.response?.status === 402) setPortalTariff();
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, [isAccountsPage, location]);
};

export default useAccounts;
