import React from "react";
import { connect } from "react-redux";
import { withTranslation } from 'react-i18next';

class Customization extends React.Component {

   render() {
      console.log("CustomizationSettings render");
      return (
         <div>
            Customization settings
         </div>
      );
   }
};

function mapStateToProps(state) {
   return {
      language: state.auth.user.cultureName || state.auth.settings.culture,
   };
}

export default connect(mapStateToProps)(withTranslation()(Customization));
