import React from "react";
import PropTypes from "prop-types";
import EnterpriseLoader from "./sub-components/EnterpriseLoader";
import TrialLoader from "./sub-components/TrialLoader";
const PaymentsStandaloneLoader = ({ isEnterprise, ...rest }) => {
  return <>{isEnterprise ? <EnterpriseLoader {...rest} /> : <TrialLoader />}</>;
};

PaymentsStandaloneLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  isEnterprise: PropTypes.bool,
};

PaymentsStandaloneLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
  isEnterprise: false,
};

export default PaymentsStandaloneLoader;
