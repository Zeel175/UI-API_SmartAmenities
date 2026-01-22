/* eslint-disable */
import { FuseNavigationItem } from '@fuse/components/navigation';

// Masters Section
// const adminChildren: FuseNavigationItem[] = [
//     {
//         id: 'user',
//         title: 'User',
//         type: 'basic',
//         icon: 'heroicons_outline:user',
//         link: '/user'
//     },
//     {
//         id: 'role',
//         title: 'Role',
//         type: 'basic',
//         icon: 'heroicons_outline:shield-check',
//         link: '/role'
//     },
//     {
//         id: 'permission',
//         title: 'Permission',
//         type: 'basic',
//         icon: 'heroicons_outline:key',
//         link: '/permission'
//     },
//     {
//         id: 'location',
//         title: 'Location',
//         type: 'basic',
//         icon: 'heroicons_outline:map-pin',
//         link: '/location'
//     },
//     {
//         id: 'module',
//         title: 'Module',
//         type: 'basic',
//         icon: 'heroicons_outline:archive-box', // You can use any heroicons name you like
//         link: '/module'
//     },
//     {
//         id: 'module-group',
//         title: 'Module Group',
//         type: 'basic',
//         icon: 'heroicons_outline:rectangle-group', // Or squares-2x2
//         link: '/module-group'
//     },

// ];

const masterChildren: FuseNavigationItem[] = [
    {
        id: 'user',
        title: 'User',
        type: 'basic',
        icon: 'heroicons_outline:user',
        link: '/user'
    },
    {
        id: 'role',
        title: 'Role',
        type: 'basic',
        icon: 'heroicons_outline:shield-check',
        link: '/role'
    },
    {
        id: 'property',
        title: 'Property',
        type: 'basic',
        icon: 'heroicons_outline:building-office',
        link: '/property'
    },
    {
        id: 'building',
        title: 'Building',
        type: 'basic',
        icon: 'heroicons_outline:building-library',
        link: '/building'
    },
    {
        id: 'device',
        title: 'Device',
        type: 'basic',
        icon: 'heroicons_outline:cpu-chip',
        link: '/device'
    },
    {
        id: 'floor',
        title: 'Floor',
        type: 'basic',
        icon: 'heroicons_outline:rectangle-stack',
        link: '/floor'
    },
    {
        id: 'zone',
        title: 'Zone',
        type: 'basic',
        icon: 'heroicons_outline:map',
        link: '/zone'
    },
    {
        id: 'unit',
        title: 'Unit',
        type: 'basic',
        icon: 'heroicons_outline:home-modern',
        link: '/unit'
    },
    {
        id: 'resident-master',
        title: 'Resident',
        type: 'basic',
        icon: 'heroicons_outline:user-group',
        link: '/resident-master'
    },
    {
        id: 'guest-master',
        title: 'Guest',
        type: 'basic',
        icon: 'heroicons_outline:user-plus',
        link: '/guest-master'
    },
    {
        id: 'amenity-master',
        title: 'Amenity',
        type: 'basic',
        icon: 'heroicons_outline:squares-plus',
        link: '/amenity-master'
    },
    {
        id: 'amenity-slot-template',
        title: 'Amenity Slot Template',
        type: 'basic',
        icon: 'heroicons_outline:calendar-days',
        link: '/amenity-slot-template'
    },
    {
        id: 'booking-header',
        title: 'Booking Header',
        type: 'basic',
        icon: 'heroicons_outline:clipboard-document-check',
        link: '/booking-header'
    },
    {
        id: 'audit-log',
        title: 'Audit Log',
        type: 'basic',
        icon: 'heroicons_outline:clipboard-document-list',
        link: '/audit-log'
    },
    {
        id: 'group-code',
        title: 'Miscellaneous',
        type: 'basic',
        icon: 'heroicons_outline:swatch',// You can use any icon you like
        link: '/group-code'
    },
    // {
    //     id: 'product',
    //     title: 'Product',
    //     type: 'basic',
    //     icon: 'heroicons_outline:cube', // You can use any icon you like
    //     link: '/product'
    // },
    // {
    //     id: 'group-code',
    //     title: 'Miscellaneous',
    //     type: 'basic',
    //     icon: 'heroicons_outline:swatch',// You can use any icon you like
    //     link: '/group-code'
    // },
    // {
    //     id: 'terms',
    //     title: 'Terms And Conditions',
    //     type: 'basic',
    //     icon: 'heroicons_outline:document-text', // You can use any icon you like
    //     link: '/terms'
    // },
    // {
    //     id: 'inventory-type',
    //     title: 'Inventory Type',
    //     type: 'basic',
    //     icon: 'heroicons_outline:clipboard-document-list', // You can use any icon you like
    //     link: '/inventory-type'
    // },

    // {
    //     id: 'process',
    //     title: 'Process',
    //     type: 'basic',
    //     icon: 'heroicons_outline:command-line', // or 'heroicons_outline:cog-6-tooth' or any icon you like
    //     link: '/process'
    // },
    // {
    //     id: 'machine',
    //     title: 'Machine',
    //     type: 'basic',
    //     icon: 'heroicons_outline:cog', // You can use any icon you like
    //     link: '/machine'
    // },
    // {
    //     id: 'vendor',
    //     title: 'Vendor',
    //     type: 'basic',
    //     icon: 'heroicons_outline:user-group', // You can use any icon you like
    //     link: '/vendor'
    // },
    // {
    //     id: 'product-group',
    //     title: 'Product Group',
    //     type: 'basic',
    //     icon: 'heroicons_outline:rectangle-group', // You can use any icon you like
    //     link: '/product-group'
    // },
];
// const inventoryChildren: FuseNavigationItem[] = [
//     {
//         id: 'sales-order',
//         title: 'Sales Order',
//         type: 'basic',
//         icon: 'heroicons_outline:receipt-percent', // You can use any icon you like
//         link: '/sales-order'
//     },
//     {
//         id: 'stockledger',
//         title: 'Stock Ledger',
//         type: 'basic',
//         icon: 'heroicons_outline:table-cells', // You can use any icon you like
//         link: '/stockledger'
//     },
//     {
//         id: 'inward',
//         title: 'Inward',
//         type: 'basic',
//         icon: 'heroicons_outline:arrow-down-tray', // You can use any icon you like
//         link: '/inward'
//     },
//     {
//         id: 'outward',
//         title: 'Outward',
//         type: 'basic',
//         icon: 'heroicons_outline:arrow-up-tray', // You can use any icon you like
//         link: '/outward'
//     },
//      {
//         id: 'inspection',
//         title: 'Inspection',
//         type: 'basic',
//         icon: 'heroicons_outline:table-cells', // You can use any icon you like
//         link: '/inspection'
//     },
//       {
//         id: 'materialRequisition',
//         title: 'Material Requisition',
//         type: 'basic',
//         icon: 'heroicons_outline:table-cells', // You can use any icon you like
//         link: '/materialRequisition'
//     },

// ];
// const productionChildren: FuseNavigationItem[] = [
//     {
//         id: 'productionWorkOrder',
//         title: 'Production Work Order',
//         type: 'basic',
//         icon: 'heroicons_outline:clipboard-document-check', // You can use any icon you like
//         link: '/productionworkorder'
//     },
//     {
//         id: 'productionplanning',
//         title: 'Production Planning',
//         type: 'basic',
//         icon: 'heroicons_outline:clipboard-document-check', // You can use any icon you like
//         link: '/productionplanning'
//     },
//     {
//         id: 'bomprocess',
//         title: 'BOM Process',
//         type: 'basic',
//         icon: 'heroicons_outline:adjustments-horizontal', // You can use any icon you like
//         link: '/bomprocess'
//     },
//     {
//         id: 'monthly-production-planning',
//         title: 'Monthly Production Planning',
//         type: 'basic',
//         icon: 'heroicons_outline:calendar-days', // You can use any icon you like
//         link: '/monthly-production-planning'
//     }, {
//         id: 'productionWorkDone',
//         title: 'Production Work Done',
//         type: 'basic',
//         icon: 'heroicons_outline:pencil-square', // You can use any icon you like
//         link: '/production-work-done'
//     },
//     {
//         id: 'in-process-qa',
//         title: 'In Process QA',
//         type: 'basic',
//         icon: 'heroicons_outline:clipboard-document-check', // You can use any icon you like
//         link: '/in-process-qa'
//     },

// ];

// const purchaseChildren: FuseNavigationItem[] = [
//     {
//         id: 'purchaseinquiry',
//         title: 'Purchase Inquiry',
//         type: 'basic',
//         icon: 'heroicons_outline:pencil-square', // You can use any icon you like
//         link: '/purchase-inquiry'
//     },
//     // {
//     //     id: 'purchaseQuotation',
//     //     title: 'Purchase Quotation',
//     //     type: 'basic',
//     //     icon: 'heroicons_outline:clipboard-document-check', // You can use any icon you like
//     //     link: '/productionplanning'
//     // },
//     {
//         id: 'purchasePlanning',
//         title: 'Purchase Planning',
//         type: 'basic',
//         icon: 'heroicons_outline:clock', // You can use any icon you like
//         link: '/purchaseplanning'
//     },
//     {
//         id: 'purchaseorder',
//         title: 'Purchase Order',
//         type: 'basic',
//         icon: 'heroicons_outline:calendar-days', // You can use any icon you like
//         link: '/purchaseorder'
//     }, {
//         id: 'purchaseOrderPlanning',
//         title: 'Purchase Order Planning ',
//         type: 'basic',
//         icon: 'heroicons_outline:pencil-square', // You can use any icon you like
//         link: '/purchase-order-planning'
//     },


// ];

// Full Navigation Structures
export const defaultNavigation: FuseNavigationItem[] = [
    {
        id: 'master',
        title: 'Master',
        type: 'collapsable',
        icon: 'heroicons_outline:arrows-up-down',
        children: masterChildren
    },
    {
        id: 'hikvision-logs',
        title: 'Hikvision Logs',
        type: 'basic',
        icon: 'heroicons_outline:video-camera',
        link: '/hikvision-logs'
    },
];

export const compactNavigation: FuseNavigationItem[] = [
    {
        id: 'master',
        title: 'Master',
        type: 'collapsable',
        icon: 'heroicons_outline:arrows-up-down',
        children: masterChildren
    },
    {
        id: 'hikvision-logs',
        title: 'Hikvision Logs',
        type: 'basic',
        icon: 'heroicons_outline:video-camera',
        link: '/hikvision-logs'
    },
];

export const futuristicNavigation: FuseNavigationItem[] = [
    {
        id: 'master',
        title: 'Master',
        type: 'collapsable',
        icon: 'heroicons_outline:arrows-up-down',
        children: masterChildren
    },
    {
        id: 'hikvision-logs',
        title: 'Hikvision Logs',
        type: 'basic',
        icon: 'heroicons_outline:video-camera',
        link: '/hikvision-logs'
    },
];

export const horizontalNavigation: FuseNavigationItem[] = [
    {
        id: 'master',
        title: 'Master',
        type: 'collapsable',
        icon: 'heroicons_outline:arrows-up-down',
        children: masterChildren
    },
    {
        id: 'hikvision-logs',
        title: 'Hikvision Logs',
        type: 'basic',
        icon: 'heroicons_outline:video-camera',
        link: '/hikvision-logs'
    },
];
