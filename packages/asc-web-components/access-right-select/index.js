import React, { useState } from "react";
import PropTypes from "prop-types";
import LinkWithDropdown from "../link-with-dropdown/index.js";
import AccessRightItem from "./sub-components/access-right-item.js";

const AccessRightSelect = ({ accessRightsList, quotaList, ...props }) => {
  const [currentItem, setCurrentItem] = useState(accessRightsList[6]);

  const formatToAccessRightItem = (data) => {
    return data.map((it) => {
      const itQuota = quotaList.find((elem) => elem.accessRightKey == it.key);
      return it.isSeparator
        ? { ...it }
        : {
            key: it.key,
            children: (
              <AccessRightItem
                key={it.key}
                title={it.title}
                description={it.description}
                icon={it.icon}
                quota={itQuota}
              />
            ),
            onClick: () => setCurrentItem(it),
          };
    });
  };

  return (
    <div style={{ display: "flex" }}>
      <img src={currentItem?.icon} style={{ marginRight: "4.18px" }} />
      <LinkWithDropdown data={formatToAccessRightItem(accessRightsList)}>
        {currentItem?.title}
      </LinkWithDropdown>
    </div>
  );
};

AccessRightSelect.propTypes = {
  /** List of rights */
  accessRightsList: PropTypes.arrayOf(PropTypes.object),
  /** List of quotas */
  quotaList: PropTypes.arrayOf(PropTypes.object),
};

export default AccessRightSelect;
