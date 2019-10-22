import React from "react";
import { connect } from "react-redux";
import { withTranslation } from 'react-i18next';

class Common extends React.Component {

 render() {
  //console.log("CommonSettings render");
  return (
   <div>
    Common settings
     </div>
  );
 }
};

function mapStateToProps(state) {
 return {
  language: state.auth.user.cultureName || state.auth.settings.culture,
 };
}

export default connect(mapStateToProps)(withTranslation()(Common));
