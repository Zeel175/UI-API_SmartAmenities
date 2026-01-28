// import { Workflow } from "@app-models";
import { environment } from "environments/environment";

const serverPath = environment.serverPath;
const apiPath = serverPath + 'api/';
let basePath = apiPath;

export const APIConstant = {
  basePath: serverPath,

  //Upload
  upload: `${apiPath}Upload`,
  base64File: `${apiPath}upload/GetBase64FromURL`,
  uploadFile: `${basePath}Users/uploadFile`,
  emailSend: `${basePath}Mail/Email`,
  allProductsInProcesses: `${apiPath}Process/AllProducts`,

  //Login and Registration
  login: `${apiPath}account/login`,
  changepassword: `${apiPath}account/change-passwor`,
  logout: `${apiPath}account/logout`,
  groupcode: `${basePath}groupcode`,
  account: `${basePath}account`,
  role: `${basePath}role`,
  activeroles: `${basePath}Role/GetActiveRoles`,
  picture: `${apiPath}Picture`,
  loginbytoken: `${apiPath}account/getuserloginbytoken`,
  modulegroup: `${basePath}modulegroup`,
  termsAndConditions: `${basePath}TermsAndConditions`,

  addUser: `${basePath}account/Add`,
  machine: `${apiPath}Machine`,
  productgroup: `${basePath}productgroup`,
  stockLedger: `${apiPath}StockLedger`,
  productionplanning: `${apiPath}ProductionPlanning`,
  approval: `${apiPath}Approval`,
  productionworkorder: `${apiPath}ProductionWorkOrder`,
  productionworkdone: `${apiPath}ProductionWorkDone`,
  bomprocess: `${apiPath}BOM`,
  inspection: `${apiPath}Inspection`,

  //WeightCheck
  // weightcheckList: `${basePath}WeightCheck/GetAllWeightCheck`,
  // weightcheckAdd: `${basePath}WeightCheck/AddWeighCheck`,
  // weightcheckEdit: `${basePath}WeightCheck/EditWeighCheck`,
  // weightcheckGetById: `${basePath}WeightCheck/GetByIdAsync`,
  // weightcheckDelete: `${basePath}WeightCheck/DeleteWeightCheck`,
  // packSizeList: `${basePath}PacksizeMaster/GetAllPacksize`,

  //AttributeCheck
  // attributecheckList: `${basePath}AttributeCheck/GetAllAttributeCheck`,
  // attributecheckAdd: `${basePath}AttributeCheck/AddAttributeCheck`,
  // attributecheckEdit: `${basePath}AttributeCheck/EditAttributeCheck`,
  // attributecheckGetById: `${basePath}AttributeCheck/GetByIdAsync`,
  // attributecheckDelete: `${basePath}AttributeCheck/Delete`,

  // DowntimeTracking
  // downtimeTrackingList: `${basePath}DowntimeTracking/GetAllDowntimeTrackings`,
  // downtimeTrackingAdd: `${basePath}DowntimeTracking/AddDowntimeTracking`,
  // downtimeTrackingEdit: `${basePath}DowntimeTracking/EditDowntimeTracking`,
  // downtimeTrackingGetById: `${basePath}DowntimeTracking/GetByIdAsync`,
  // downtimeTrackingDelete: `${basePath}DowntimeTracking/DeleteDowntimeTracking`,

  // Permission API endpoints
  permissionList: `${basePath}RolePermission/GetAllRolePermissions`,
  permissionAdd: `${basePath}RolePermission/AddRolePermission`,
  permissionEdit: `${basePath}RolePermission/UpdateRolePermission`,
  permissionGetById: `${basePath}RolePermission/GetById`,
  permissionDelete: `${basePath}RolePermission/Delete`,
  permission: `${basePath}Permission/GetAllPermissions`,
  permissionListPaged: `${basePath}RolePermission/GetAllRolePermissionsPaged`,

  // PalletPacking
  // palletPackingList: `${basePath}PalletPacking/GetAllPalletPacking`,
  // palletPackingAdd: `${basePath}PalletPacking/AddPalletPacking`,
  // palletPackingEdit: `${basePath}PalletPacking/EditPalletPacking`,
  // palletPackingGetById: `${basePath}PalletPacking/GetByIdAsync`,
  // palletPackingDelete: `${basePath}PalletPacking/DeletePalletPacking`,
  //Trailer Inspection
  // TrailerInspectionList: `${basePath}TrailerInspection/GetAllTrailerInspection`,
  // TrailerInspectionAdd: `${basePath}TrailerInspection/AddTrailerInspection`,
  // TrailerInspectionEdit: `${basePath}TrailerInspection/EditTrailerInspection`,
  // TrailerInspectionGetById: `${basePath}TrailerInspection/GetByIdAsync`,
  // TrailerInspectionDelete: `${basePath}TrailerInspection/DeleteTrailerInspection`,

  // Trailer Loading
  // trailerLoadingList: `${basePath}TrailerLoading/GetAllTrailerLoadings`,
  // trailerLoadingAdd: `${basePath}TrailerLoading/AddTrailerLoading`,
  // trailerLoadingEdit: `${basePath}TrailerLoading/EditTrailerLoading`,
  // trailerLoadingGetById: `${basePath}TrailerLoading/GetByIdAsync`,
  // trailerLoadingDelete: `${basePath}TrailerLoading/DeleteTrailerLoading`,

  //laborVariance
  laborVarinceList: `${basePath}LaborVariance/GetAllLaborVariance`,
  laborVarinceAdd: `${basePath}LaborVariance/AddLaborVariance`,
  laborVarinceEdit: `${basePath}LaborVariance/EditLaborVariance`,
  laborVarinceGetById: `${basePath}LaborVariance/GetByIdAsync`,
  laborVarinceDelete: `${basePath}LaborVariance/DeleteLaborVariance`,

  //Property
  PropertyMasterList: `${basePath}Property/GetAllProperty`,  //TrailerInspection service
  PropertyMasterGetById: `${basePath}Property/GetByIdAsync`,
  PropertyMasterAdd: `${basePath}Property/AddProperty`,
  PropertyMasterEdit: `${basePath}Property/EditProperty`,
  PropertyMasterDelete: `${basePath}Property/DeleteProperty`,
  PropertyBasicList:`${basePath}Property/GetAllPropertyBasic`,
  //Zone
  ZoneMasterList: `${basePath}Zone/GetAllZone`,
  ZoneMasterGetById: `${basePath}Zone/GetByIdAsync`,
  ZoneMasterAdd: `${basePath}Zone/AddZone`,
  ZoneMasterEdit: `${basePath}Zone/EditZone`,
  ZoneMasterDelete: `${basePath}Zone/DeleteZone`,
  ZoneBasicList: `${basePath}Zone/GetAllZoneBasic`,
  //Building
    BuildingMasterList: `${basePath}Building/GetAllBuilding`,
    BuildingMasterGetById: `${basePath}Building/GetByIdAsync`,
    BuildingMasterAdd: `${basePath}Building/AddBuilding`,
    BuildingMasterEdit: `${basePath}Building/EditBuilding`,
    BuildingMasterDelete: `${basePath}Building/DeleteBuilding`,
    HikvisionDevicesList: `${basePath}Hikvision/devices`,
    HikvisionLogsSearch: `${basePath}HikvisionLogs/Search`,
    //Resident Master
    ResidentMasterListPaged: `${basePath}ResidentMaster/List/paged`,
    ResidentMasterGetById: `${basePath}ResidentMaster/GetById`,
    ResidentMasterAdd: `${basePath}ResidentMaster/Create`,
    ResidentMasterEdit: `${basePath}ResidentMaster/Update`,
    ResidentMasterDelete: `${basePath}ResidentMaster/Delete`,
    ResidentMasterDocuments: `${basePath}ResidentMaster/Documents`,
    ResidentMasterDocumentDelete: `${basePath}ResidentMaster/Documents/Delete`,
    //Guest Master
    GuestMasterListPaged: `${basePath}GuestMaster/List/paged`,
    GuestMasterGetById: `${basePath}GuestMaster/GetById`,
    GuestMasterAdd: `${basePath}GuestMaster/CreateGuest`,
    GuestMasterEdit: `${basePath}GuestMaster/EditGuest`,
    GuestMasterDelete: `${basePath}GuestMaster/DeleteGuest`,
    GuestMasterDocuments: `${basePath}GuestMaster/Documents`,
    GuestMasterDocumentDelete: `${basePath}GuestMaster/Documents/Delete`,
    GuestMasterByUnit: `${basePath}GuestMaster/List/by-unit`,

    //Amenity Master
    AmenityMasterListPaged: `${basePath}AmenityMaster/GetAllAmenity/paged`,
    AmenityMasterGetById: `${basePath}AmenityMaster/GetByIdAsync`,
    AmenityMasterAdd: `${basePath}AmenityMaster/AddAmenity`,
    AmenityMasterEdit: `${basePath}AmenityMaster/EditAmenity`,
    AmenityMasterDelete: `${basePath}AmenityMaster/DeleteAmenity`,
    AmenityMasterDocumentDelete: `${basePath}AmenityMaster/Documents/Delete`,
    AmenityMasterBasicList: `${basePath}AmenityMaster/GetAllAmenityBasic`,

    //Amenity Unit Master
    AmenityUnitListPaged: `${basePath}AmenityUnitMaster/GetAllAmenityUnit/paged`,
    AmenityUnitByAmenityId: `${basePath}AmenityUnitMaster/GetByAmenityId`,
    AmenityUnitGetById: `${basePath}AmenityUnitMaster/GetByIdAsync`,
    AmenityUnitAdd: `${basePath}AmenityUnitMaster/AddAmenityUnit`,
    AmenityUnitEdit: `${basePath}AmenityUnitMaster/EditAmenityUnit`,
    AmenityUnitDelete: `${basePath}AmenityUnitMaster/DeleteAmenityUnit`,

    //Amenity Slot Template
    AmenitySlotTemplateListPaged: `${basePath}AmenitySlotTemplate/GetAllSlotTemplate/paged`,
    AmenitySlotTemplateGetById: `${basePath}AmenitySlotTemplate/GetByIdAsync`,
    AmenitySlotTemplateAdd: `${basePath}AmenitySlotTemplate/AddSlotTemplate`,
    AmenitySlotTemplateAddBulk: `${basePath}AmenitySlotTemplate/AddSlotTemplate/bulk`,
    AmenitySlotTemplateEdit: `${basePath}AmenitySlotTemplate/EditSlotTemplate`,
    AmenitySlotTemplateEditBulk: `${basePath}AmenitySlotTemplate/EditSlotTemplate/bulk`,
    AmenitySlotTemplateDelete: `${basePath}AmenitySlotTemplate/DeleteSlotTemplate`,

    //Booking Header
    BookingHeaderListPaged: `${basePath}BookingHeader/GetAllBooking/paged`,
    BookingHeaderGetById: `${basePath}BookingHeader/GetByIdAsync`,
    BookingHeaderAdd: `${basePath}BookingHeader/AddBooking`,
    BookingHeaderEdit: `${basePath}BookingHeader/EditBooking`,
    BookingHeaderDelete: `${basePath}BookingHeader/DeleteBooking`,
    BookingHeaderAvailableSlots: `${basePath}BookingHeader/AvailableSlots`,

    //Floor
    FloorMasterList: `${basePath}Floor/GetAllFloor`,
    FloorMasterGetById: `${basePath}Floor/GetByIdAsync`,
    FloorMasterAdd: `${basePath}Floor/AddFloor`,
    FloorMasterEdit: `${basePath}Floor/EditFloor`,
    FloorMasterDelete: `${basePath}Floor/DeleteFloor`,
    BuildingBasicList: `${basePath}Building/GetAllBuildingBasic`,
    FloorBasicList: `${basePath}Floor/GetAllFloorBasic`,
    // Unit
    UnitMasterList: `${basePath}Unit/GetAllUnit`,
    UnitMasterListBasic: `${basePath}Unit/GetAllUnitBasic`,
    UnitMasterGetById: `${basePath}Unit/GetByIdAsync`,
    UnitMasterAdd: `${basePath}Unit/AddUnit`,
    UnitMasterEdit: `${basePath}Unit/EditUnit`,
    UnitMasterDelete: `${basePath}Unit/DeleteUnit`,
    UnitByBuilding: `${basePath}Unit/GetUnitsByBuilding`,
    UsersByUnit: `${basePath}ResidentMaster/GetUsersByUnit`,
  //auditLog
  AuditLogList: `${basePath}AuditLog/GetAllAuditLogs`,

  //GroupCode
  GroupCodeList: `${basePath}GroupCode/GetAllGroupCodes`,
  GroupCodeListByGroupName: `${basePath}GroupCode/list`,
  //Masters
  // VehicleTypeMasterList: `${basePath}Masters/vehicle-types`,  //TrailerInspection service 

  //Nozzle
  // NozzleList: `${basePath}NozzleMaster/GetAllNozzle`,  //WeightCHeck service 

  //ProductionOrder
  ProductionOrderList: `${basePath}ProductionOrder/GetAllProductionOrder`,  //WeightCHeck service 

  //Product
  ProductList: `${basePath}ProductMaster/GetAllProduct`,  //WeightCHeck service 

  //User
  UserList: `${basePath}users`,  //WeightCHeck service 
  UserNameAndId: `${basePath}users/UserNamesAndIds`,

  //Shift
  ShiftList: `${basePath}ShiftMaster/GetAllShift`,  //WeightCHeck service 
  CauseList: `${basePath}CauseMaster/GetAllCauses`,
  MasterList: `${apiPath}Masters/GetAllMasters`,

  // Pre-Check 
  // preCheckList: `${basePath}PreCheckList/GetAllPreCheckList`,
  // preCheckAdd: `${basePath}PreCheckList/AddPreCheckList`,
  // preCheckEdit: `${basePath}PreCheckList/EditPreCheckList`,
  // preCheckGetById: `${basePath}PreCheckList/GetByIdAsync`,
  // preCheckDelete: `${basePath}PreCheckList/Delete`,

  // Post-Check 
  // postCheckList: `${basePath}PostCheckList/GetAllPostCheckList`,
  // postCheckAdd: `${basePath}PostCheckList/AddPostCheckList`,
  // postCheckEdit: `${basePath}PostCheckList/EditPostCheckList`,
  // postCheckGetById: `${basePath}PostCheckList/GetByIdAsync`,
  // postCheckDelete: `${basePath}PostCheckList/Delete`,


  // Production Order APIs
  productionOrderList: `${basePath}ProductionOrder/GetAllProductionOrder`,
  productionOrderGetById: `${basePath}ProductionOrder/GetByIdAsync`,
  productionOrderStatus: `${basePath}ProductionOrder/ToggleStatus`,
  POByStatus: `${basePath}ProductionOrder/GetPOByStatus`,


  // PrePostQuestion
  PrePostQuestion: `${basePath}PrePostQuestion/GetAllPrePostQuestions`,

  //Masters 
  StartEndBatchChecklist: `${basePath}StartEndBatchChecklist/GetAllStartEndBatchChecklist`,
  GetAllProductInstructionDetails: `${basePath}ProductInstructionDetails/GetAllProductInstructionDetails`,
  GetAllMaterialMaster: `${basePath}MaterialMaster/GetAllMaterialMaster`,
  GetAllMixingTankMaster: `${basePath}MixingTankMaster/GetAllMixingTankMaster`,
  GetAllStorageTankMaster: `${basePath}StorageTankMaster/GetAllStorageTankMaster`,
  GetAllQCTSpecificationMaster: `${basePath}QCTSpecificationMaster/GetAllQCTSpecificationMaster`,
  GetProductInstructionDetailsByProductId: `${basePath}ProductInstructionDetails/GetProductInstructionDetailsById`,
  GetProductQCTSpecificationById: `${basePath}QCTSpecificationMaster/GetProductQCTSpecificationById`,


  //LiquidPreparation
  // LiquidPreparationList: `${basePath}LiquidPreparation/GetAllLiquidPreparation`,
  // LiquidPreparationAdd: `${basePath}LiquidPreparation/AddLiquidPreparation`,
  // LiquidPreparationEdit: `${basePath}LiquidPreparation/EditLiquidPreparation`,
  // LiquidPreparationGetById: `${basePath}LiquidPreparation/GetByIdAsync`,
  // LiquidPreparationDelete: `${basePath}LiquidPreparation/DeleteLiquidPreparation`,

  //Pages
  company: `${apiPath}company`,
  user: `${apiPath}users`,
  employee: `${apiPath}ExternalEmployees`,
  country: `${apiPath}country`,
  state: `${apiPath}state`,
  module: `${apiPath}module`,
  student: `${apiPath}Student`,
  bom: `${basePath}bommaster`,
  customer: `${basePath}customer`,
  process: `${basePath}process`,
  product: `${basePath}product`,
  salesOrder: `${basePath}salesorder`,
  vendormaster: `${basePath}vendormaster`,
  workflow: `${basePath}workflow`,
  purchaseOrder: `${basePath}purchaseorder`,
  purchaseOrderPlanning: `${basePath}purchaseorderplanning`,
  inventorytype: `${basePath}inventorytype`,
  inward: `${basePath}inward`,
  outward: `${basePath}outward`,
  qualityParameter: `${basePath}qualityparameter`,
  materialRequisition: `${basePath}materialrequisition`,
  location: `${basePath}location`,
  //status: `${basePath}status`,
  groupLineItem: `${basePath}ParentGroup`,
  groupLineItemDelete: `${basePath}ParentGroup/DeleteEntities`,
  deleteParentGroup: `${basePath}ParentGroup/deleteParentGroup`,
  legacylineitems: `${basePath}LegacyLineItems`,
  designation: `${basePath}Designation`,
  designationDelete: `${basePath}Designation/DeleteDesignation`,
  projectDelete: `${basePath}ProjectMaster/DeleteProject`,
  transaction: `${basePath}transaction`,
  projectMaster: `${basePath}ProjectMaster`,
  externalProject: `${basePath}ExternalProjects`,
  provisionalInputScreen: `${basePath}provisionalinputscreen`,
  projectAssignment: `${basePath}ProjectAssignment`,
  projectAssignmentApproval: `${basePath}ProjectAssignmentApproval`,
  serviceCategory: `${basePath}servicecategory`,
  budgetEntryScreen: `${basePath}BudgetMaster`,
  budgetEntryScreenDelete: `${basePath}BudgetMaster/DeleteBudget`,
  budgetGetFileDataById: `${basePath}BudgetMaster/GetFileDataById`,
  erpbudget: `${basePath}ERPBudget`,
  budgetapprover: `${basePath}BudgetApprover`,
  actualinputapprover: `${basePath}ActualInputScreenApproval`,
  provisionalinputapprover: `${basePath}ProvisionalInputScreenApproval`,
  actualGetFileDataById: `${basePath}actualinputscreen/GetFileDataById`,
  provisionalGetFileDataById: `${basePath}ProvisionalInputScreen/GetFileDataById`,

  actualInputScreen: `${basePath}actualinputscreen`,
  budgetAttachmentDownload: `${basePath}BudgetMaster/Download`,
  city: `${basePath}city`,
  actualInputAttachmentDownload: `${basePath}ActualInputScreen/Download`,
  getUsersByProjectAssignmentId: (id: number) => `${basePath}ProjectAssignment/GetUsersByProjectAssignmentId/${id}`,
  provisionalInputAttachmentDownload: `${basePath}ProvisionalInputScreen/Download`,
  InProcessQA: `${apiPath}InProcessQA`,

  PurchaseInquiry: `${apiPath}PurchaseInquiry`,
  purchaseplanning: `${apiPath}PurchasePlanning`,

  //List
  list: {
    modulePermission: `${apiPath}module`,
    role: `${apiPath}role/list`,
    town: `${basePath}parkingauthority/list`,
    onlinetheme: `${basePath}groupcode/list/onlinetheme`,
    locationgrouptype: `${basePath}groupcode/list/locationgrouptype`,
    authenticationtype: `${basePath}groupcode/list/authenticationtype`,
    groupnames: `${basePath}groupcode/groups`,
    names: `${basePath}groupcode/groupname`,
    types: `${apiPath}groupcode/list/types`,
    categories: `${apiPath}groupcode/list/categories`,
    customercategory: `${apiPath}groupcode/list/customercategory`,
    industries: `${apiPath}groupcode/list/industries`,
    sources: `${apiPath}groupcode/list/sources`,
    customertype: `${apiPath}groupcode/list/customertype`,
    territories: `${apiPath}groupcode/list/territories`,
    productGroups: `${apiPath}groupcode/list/productGroups`,
    productCategories: `${apiPath}groupcode/list/productCategories`,
    productTypes: `${apiPath}groupcode/list/productTypes`,
    productBrands: `${apiPath}groupcode/list/productBrands`,
    productSizes: `${apiPath}groupcode/list/productSizes`,
    typeOfConnections: `${apiPath}groupcode/TypeOfConnection`,
    interStateTaxes: `${apiPath}groupcode/list/interStateTaxes`,
    intraStateTaxes: `${apiPath}groupcode/list/intraStateTaxes`,
    uoms: `${apiPath}groupcode/list/uom`,
    customers: `${basePath}customer/list`,
    salesOrderTypes: `${apiPath}groupcode/list/salesOrderTypes`,
    currencies: `${apiPath}groupcode/list/currencies`,
    products: `${apiPath}product/list`,
    discountTypes: `${apiPath}groupcode/list/discountTypes`,
    productcategories: `${apiPath}groupcode/list/productcategories`,
    approvertypes: `${apiPath}groupcode/list/approvertypes`,
    vendors: `${apiPath}vendormaster/list`,
    approvalStatuses: `${apiPath}groupcode/list/approvalStatus`,
    termsConditions: `${basePath}TermsAndConditions/list`,
    finishedproducts: `${apiPath}product/finishedProducts`,
    qcparameters: `${apiPath}groupcode/list/qcparameters`,
    qcclasses: `${apiPath}groupcode/list/qcclasses`,
    lslDimensions: `${apiPath}groupcode/list/lslDimensions`,
    uslDimensions: `${apiPath}groupcode/list/uslDimensions`,
    measuringInstruments: `${apiPath}groupcode/list/measuringInstruments`,
    sampleFrequencies: `${apiPath}groupcode/list/sampleFrequencies`,
    reportingFrequencies: `${apiPath}groupcode/list/reportingFrequencies`,
    warehouses: `${apiPath}location/warehouses`,
    rawmaterialProducts: `${apiPath}product/rawmaterialProducts`,
    companies: `${apiPath}company/List`,
    process: `${apiPath}Process`,
    status: `${apiPath}groupcode/list/Status`,
    colours: `${apiPath}groupcode/list/Colour`,
    packings: `${apiPath}groupcode/list/Packing`,

  },
}

export const PublicAPI = [
  APIConstant.login, APIConstant.loginbytoken
]
